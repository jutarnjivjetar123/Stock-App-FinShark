using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using api.Repository.CommentRepository;
using api.Interfaces.ICommentRepository;
using api.Mappers.CommentMappers;
using api.Dtos.Comment.CreateCommentDto;
using api.Dtos.Comment.UpdateCommentDto;
using api.Repository.StockRepository;
using api.Interfaces.IStockRepository;

namespace api.Controllers.CommentController
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController  : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;
        public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository)
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() {
            var comments = await _commentRepository.GetAllAsync();
            var commentDto = comments.Select(x => x.ToCommentDto());
            return Ok(commentDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute
        ] int id)
        {
            
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null) {
                return NotFound("Comment not found");
            }
            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDto commentDto) {

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (!await _stockRepository.StockExists(stockId)) {
                return BadRequest("Stock does not exist");
            }

            var comment = commentDto.ToCommentFromCreateCommentDto(stockId);
            
            await _commentRepository.CreateAsync(comment);
            return CreatedAtAction(nameof(GetById), new { id = comment }, comment.ToCommentDto());

        }
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentDto updateCommentDto) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState.First().Value.ToString());
            }
            var newComment = await _commentRepository.UpdateAsync(id, updateCommentDto.ToCommentFromUpdateCommentDto(id));
            if (newComment == null) {
                return BadRequest("Comment does not exist");
            }
            return Ok(updateCommentDto);

        } 
        [HttpDelete("{commentId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int commentId) {
            var comment = await _commentRepository.DeleteAsync(commentId);
            if (comment == null) {
                return BadRequest("Comment does not exist");
            }
            return Ok("Comment was deleted");
        }
    }
}