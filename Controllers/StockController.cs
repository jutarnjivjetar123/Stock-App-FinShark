using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using api.Dtos.StockDto;
using api.Dtos.Stock.UpdateStockRequestDto;
using api.Dtos.Stock.CreateStockRequestDto;
using api.Interfaces.IStockRepository;
using api.Repository.StockRepository;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using api.Helpers;
namespace api.Controllers
{

[Route("api/stock")]
[ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IStockRepository _stockRepo;
        public StockController(ApplicationDBContext context, IStockRepository stockRepo) {
            _stockRepo = stockRepo;
            _context = context;
         }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query) {
            var stocks = await _stockRepo.GetAllAsync(query);
            if (stocks.Count() == 0) {
                var isSymbolSet = !string.IsNullOrWhiteSpace(query.Symbol);
                var isCompanyNameSet = !string.IsNullOrWhiteSpace(query.CompanyName);

                if (isSymbolSet && !isCompanyNameSet) {
                    return NotFound("No stock with symbol " + query.Symbol + " found");
                }

                if (isCompanyNameSet && !isSymbolSet) {
                    return NotFound("No stock with company name " + query.CompanyName + " found");
                }

                return NotFound("No stock with symbol " + query.Symbol + " and company name " + query.CompanyName + " found");

            }
            var stockDto = stocks.Select(s => s.ToStockDto());

            var returnObject = new
            {
                stockCount = stocks.Count,
                stocks = stockDto
            };

            return Ok(returnObject);            
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id) {

            var stock = await _stockRepo.GetByIdAsync(id);

            if (stock == null) {
                return NotFound("Stock was not found");
            }
            return Ok(stock);
         }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto) {

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var stockModel = stockDto.ToStockFromCreateDto();
            await _stockRepo.CreateAsync(stockModel);


            
            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockResponseDto());
         }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto) {
            var stockModel = await _stockRepo.UpdateAsync(id, updateDto);
            if (stockModel == null) {
                return NotFound("Stock not found");
            }

          
            return Ok(stockModel.ToStockResponseDto());
        }


        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id) {
            var stockModel = await _stockRepo.DeleteAsync(id);

            if (stockModel == null) {
                return NotFound("Stock not found");
            }
            return Ok("Stock was deleted successfully");
        }

    }
}