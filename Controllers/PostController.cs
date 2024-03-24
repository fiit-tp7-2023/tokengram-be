using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.HTTP.Responses;
using Tokengram.Services.Interfaces;

namespace Tokengram.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public partial class PostController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;

        public PostController(IMapper mapper, IPostService postService, ICommentService commentService)
        {
            _mapper = mapper;
            _postService = postService;
            _commentService = commentService;
        }

        [HttpPut("{postNFTAddress}/settings")]
        public async Task<ActionResult<BasicPostUserSettingsResponseDTO>> UpdatePostUserSettings(
            string postNFTAddress,
            PostUserSettingsRequestDTO request
        )
        {
            var result = await _postService.UpdatePostUserSettings(request, postNFTAddress, GetUserAddress());

            return Ok(_mapper.Map<BasicPostUserSettingsResponseDTO>(result));
        }

        [HttpGet("{postNFTAddress}/likes")]
        public async Task<ActionResult<IEnumerable<PostLikeResponseDTO>>> GetPostLikes(
            string postNFTAddress,
            [FromQuery] PaginationRequestDTO request
        )
        {
            var result = await _postService.GetPostLikes(request, postNFTAddress);

            return Ok(_mapper.Map<IEnumerable<PostLikeResponseDTO>>(result));
        }

        [HttpPost("{postNFTAddress}/likes")]
        public async Task<ActionResult<BasicPostLikeResponseDTO>> LikePost(string postNFTAddress)
        {
            var result = await _postService.LikePost(postNFTAddress, GetUserAddress());

            return CreatedAtAction(
                nameof(LikePost),
                new { id = result.Id },
                _mapper.Map<BasicPostLikeResponseDTO>(result)
            );
        }

        [HttpDelete("{postNFTAddress}/likes")]
        public async Task<ActionResult<BasicPostLikeResponseDTO>> UnlikePost(string postNFTAddress)
        {
            await _postService.UnlikePost(postNFTAddress, GetUserAddress());

            return NoContent();
        }
    }
}
