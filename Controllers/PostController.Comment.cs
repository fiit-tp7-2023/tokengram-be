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
        public async Task<ActionResult<IEnumerable<CommentWithUserContextResponseDTO>>> GetComments(
            string postNFTAddress,
            [FromQuery] PaginationRequestDTO request
        )
        {
            Post post = (HttpContext.Items["post"] as Post)!;

            var result = await _commentService.GetCommentsWithUserContext(request, post, GetUserAddress());

            return Ok(_mapper.Map<IEnumerable<CommentWithUserContextResponseDTO>>(result));
        }

        [HttpPost("{postNFTAddress}/comments")]
        [BindPost]
        public async Task<ActionResult<BasicCommentResponseDTO>> CreateComment(
            string postNFTAddress,
            CommentRequestDTO request
        )
        {
            Post post = (HttpContext.Items["post"] as Post)!;

            var result = await _commentService.CreateComment(request, post, GetUserAddress());

            return CreatedAtAction(
                nameof(CreateComment),
                new { id = result.Id },
                _mapper.Map<BasicCommentResponseDTO>(result)
            );
        }

        [HttpGet("{postNFTAddress}/comments/{commentId}/replies")]
        [BindComment]
        public async Task<ActionResult<IEnumerable<CommentWithUserContextResponseDTO>>> GetCommentReplies(
            string postNFTAddress,
            long commentId,
            [FromQuery] PaginationRequestDTO request
        )
        {
            Comment comment = (HttpContext.Items["ValidatedComment"] as Comment)!;

            var result = await _commentService.GetCommentRepliesWithUserContext(request, comment, GetUserAddress());

            return Ok(_mapper.Map<IEnumerable<CommentWithUserContextResponseDTO>>(result));
        }

        [HttpPut("{postNFTAddress}/comments/{commentId}")]
        [BindComment]
        public async Task<ActionResult<BasicCommentResponseDTO>> UpdateComment(
            string postNFTAddress,
            long commentId,
            CommentUpdateRequestDTO request
        )
        {
            Comment comment = (HttpContext.Items["ValidatedComment"] as Comment)!;

            var result = await _commentService.UpdateComment(request, comment, GetUserAddress());

            return Ok(_mapper.Map<BasicCommentResponseDTO>(result));
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
        public async Task<ActionResult<BasicCommentLikeResponseDTO>> LikeComment(string postNFTAddress, long commentId)
        {
            Comment comment = (HttpContext.Items["ValidatedComment"] as Comment)!;

            var result = await _commentService.LikeComment(comment, GetUserAddress());

            return CreatedAtAction(
                nameof(LikeComment),
                new { id = result.Id },
                _mapper.Map<BasicCommentLikeResponseDTO>(result)
            );
        }

        [HttpDelete("{postNFTAddress}/comments/{commentId}/likes")]
        [BindComment]
        public async Task<ActionResult<BasicCommentLikeResponseDTO>> UnlikeComment(
            string postNFTAddress,
            long commentId
        )
        {
            Comment comment = (HttpContext.Items["ValidatedComment"] as Comment)!;

            await _commentService.UnlikeComment(comment, GetUserAddress());

            return NoContent();
        }
    }
}
