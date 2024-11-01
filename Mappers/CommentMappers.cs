using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Dtos.Comment.CreateCommentDto;
using api.Dtos.Comment.UpdateCommentDto;
using api.Models;
namespace api.Mappers.CommentMappers
{
    public static class CommentMappers
    {
        public static CommentDto ToCommentDto(this Comment commentModel) {
            return new CommentDto
            {
                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                CreatedOn = commentModel.CreatedOn,
                StockId = commentModel.StockId
            };
         }


        public static Comment ToCommentFromCreateCommentDto(this CreateCommentDto commentDto, int stockId) {
            return new Comment
            {
                Title = commentDto.Title,
                Content = commentDto.Content,
                StockId = stockId
            };
        }

        public static Comment ToCommentFromUpdateCommentDto(this UpdateCommentDto updateCommentDto, int stockId) {
            return new Comment
            {
                Title = updateCommentDto.Title,
                Content = updateCommentDto.Content,
                StockId = stockId
            };
         }
    }
}