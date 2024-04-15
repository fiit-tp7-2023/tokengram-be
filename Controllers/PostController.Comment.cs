using Microsoft.AspNetCore.Mvc;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Infrastructure.ActionFilters;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.HTTP.Responses;

namespace Tokengram.Controllers
{
    public partial class PostController : BaseController
    {
        [HttpGet("{postNFTAddress}/comments")]
        [BindPost]
        public async Task<ActionResult<IEnumerable<CommentResponseDTO>>> GetComments(
            string postNFTAddress,
            [FromQuery] PaginationRequestDTO request
        )
        {
            Post post = (HttpContext.Items["post"] as Post)!;

            var result = await _commentService.GetComments(request, post, GetUserAddress());

            return Ok(_mapper.Map<IEnumerable<CommentResponseDTO>>(result));
        }

        [HttpPost("{postNFTAddress}/comments")]
        [BindPost]
        public async Task<ActionResult<CommentResponseDTO>> CreateComment(
            string postNFTAddress,
            CommentRequestDTO request
        )
        {
            Post post = (HttpContext.Items["post"] as Post)!;

            var result = await _commentService.CreateComment(request, post, GetUserAddress());

            return Created(nameof(CreateComment), _mapper.Map<CommentResponseDTO>(result));
        }

        [HttpGet("{postNFTAddress}/comments/{commentId}/replies")]
        [BindComment]
        public async Task<ActionResult<IEnumerable<CommentResponseDTO>>> GetCommentReplies(
            string postNFTAddress,
            long commentId,
            [FromQuery] PaginationRequestDTO request
        )
        {
            Comment comment = (HttpContext.Items["ValidatedComment"] as Comment)!;

            var result = await _commentService.GetCommentReplies(request, comment, GetUserAddress());

            return Ok(_mapper.Map<IEnumerable<CommentResponseDTO>>(result));
        }

        [HttpPut("{postNFTAddress}/comments/{commentId}")]
        [BindComment]
        public async Task<ActionResult<CommentResponseDTO>> UpdateComment(
            string postNFTAddress,
            long commentId,
            CommentUpdateRequestDTO request
        )
        {
            Comment comment = (HttpContext.Items["ValidatedComment"] as Comment)!;

            var result = await _commentService.UpdateComment(request, comment, GetUserAddress());

            return Ok(_mapper.Map<CommentResponseDTO>(result));
        }

        [HttpDelete("{postNFTAddress}/comments/{commentId}")]
        [BindComment]
        public async Task<ActionResult> DeleteComment(string postNFTAddress, long commentId)
        {
            Comment comment = (HttpContext.Items["ValidatedComment"] as Comment)!;

            await _commentService.DeleteComment(comment, GetUserAddress());

            return NoContent();
        }

        [HttpGet("{postNFTAddress}/comments/{commentId}/likes")]
        [BindComment]
        public async Task<ActionResult<IEnumerable<CommentLikeResponseDTO>>> GetCommentLikes(
            string postNFTAddress,
            long commentId,
            [FromQuery] PaginationRequestDTO request
        )
        {
            Comment comment = (HttpContext.Items["ValidatedComment"] as Comment)!;

            var result = await _commentService.GetCommentLikes(request, comment);

            return Ok(_mapper.Map<IEnumerable<CommentLikeResponseDTO>>(result));
        }

        [HttpPost("{postNFTAddress}/comments/{commentId}/likes")]
        [BindComment]
        public async Task<ActionResult<CommentLikeResponseDTO>> LikeComment(string postNFTAddress, long commentId)
        {
            Comment comment = (HttpContext.Items["ValidatedComment"] as Comment)!;

            var result = await _commentService.LikeComment(comment, GetUserAddress());

            return Created(nameof(LikeComment), _mapper.Map<CommentLikeResponseDTO>(result));
        }

        [HttpDelete("{postNFTAddress}/comments/{commentId}/likes")]
        [BindComment]
        public async Task<ActionResult> UnlikeComment(string postNFTAddress, long commentId)
        {
            Comment comment = (HttpContext.Items["ValidatedComment"] as Comment)!;

            await _commentService.UnlikeComment(comment, GetUserAddress());

            return NoContent();
        }
    }
}
