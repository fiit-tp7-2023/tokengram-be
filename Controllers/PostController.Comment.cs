using Microsoft.AspNetCore.Mvc;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.HTTP.Responses;

namespace Tokengram.Controllers
{
    public partial class PostController : BaseController
    {
        [HttpGet("{postNFTAddress}/comments")]
        public async Task<ActionResult<IEnumerable<CommentResponseDTO>>> GetComments(
            string postNFTAddress,
            [FromQuery] PaginationRequestDTO request
        )
        {
            var result = await _commentService.GetComments(request, postNFTAddress);

            return Ok(_mapper.Map<IEnumerable<CommentResponseDTO>>(result));
        }

        [HttpPost("{postNFTAddress}/comments")]
        public async Task<ActionResult<BasicCommentResponseDTO>> CreateComment(
            string postNFTAddress,
            CommentRequestDTO request
        )
        {
            var result = await _commentService.CreateComment(request, postNFTAddress, GetUserAddress());

            return CreatedAtAction(
                nameof(CreateComment),
                new { id = result.Id },
                _mapper.Map<BasicCommentResponseDTO>(result)
            );
        }

        [HttpGet("comments/{commentId}/replies")]
        public async Task<ActionResult<IEnumerable<CommentResponseDTO>>> GetCommentReplies(
            long commentId,
            [FromQuery] PaginationRequestDTO request
        )
        {
            var result = await _commentService.GetCommentReplies(request, commentId);

            return Ok(_mapper.Map<IEnumerable<CommentResponseDTO>>(result));
        }

        [HttpPut("comments/{commentId}")]
        public async Task<ActionResult<BasicCommentResponseDTO>> UpdateComment(
            long commentId,
            CommentUpdateRequestDTO request
        )
        {
            var result = await _commentService.UpdateComment(request, commentId, GetUserAddress());

            return Ok(_mapper.Map<BasicCommentResponseDTO>(result));
        }

        [HttpDelete("comments/{commentId}")]
        public async Task<ActionResult> DeleteComment(long commentId)
        {
            await _commentService.DeleteComment(commentId, GetUserAddress());

            return NoContent();
        }

        [HttpGet("comments/{commentId}/likes")]
        public async Task<ActionResult<IEnumerable<CommentLikeResponseDTO>>> GetCommentLikes(
            long commentId,
            [FromQuery] PaginationRequestDTO request
        )
        {
            var result = await _commentService.GetCommentLikes(request, commentId);

            return Ok(_mapper.Map<IEnumerable<CommentLikeResponseDTO>>(result));
        }

        [HttpPost("comments/{commentId}/likes")]
        public async Task<ActionResult<BasicCommentLikeResponseDTO>> LikeComment(long commentId)
        {
            var result = await _commentService.LikeComment(commentId, GetUserAddress());

            return CreatedAtAction(
                nameof(LikeComment),
                new { id = result.Id },
                _mapper.Map<BasicCommentLikeResponseDTO>(result)
            );
        }

        [HttpDelete("comments/{commentId}/likes")]
        public async Task<ActionResult<BasicCommentLikeResponseDTO>> UnlikeComment(long commentId)
        {
            await _commentService.UnlikeComment(commentId, GetUserAddress());

            return NoContent();
        }
    }
}
