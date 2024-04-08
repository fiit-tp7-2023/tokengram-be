using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Infrastructure.ActionFilters;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.HTTP.Responses;
using Tokengram.Services.Interfaces;

namespace Tokengram.Controllers
{
    [Authorize]
    [ApiController]
    [Route("posts")]
    public partial class PostController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;

        private readonly IRecommendationService _recommendationService;

        private readonly IUserService _userService;

        public PostController(
            IMapper mapper,
            IPostService postService,
            ICommentService commentService,
            IUserService userService,
            IRecommendationService recommendationService
        )
        {
            _mapper = mapper;
            _postService = postService;
            _userService = userService;
            _recommendationService = recommendationService;
            _commentService = commentService;
        }
        
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<UserPostResponseDTO>>> GetUserPosts(
            [FromQuery] PaginationRequestDTO request
        )
        {
            var result = await _postService.GetUserPosts(request, GetUserAddress());

            return Ok(_mapper.Map<IEnumerable<UserPostResponseDTO>>(result));
        }
        
        [HttpGet("hot-posts")]
        public async Task<ActionResult<IEnumerable<UserPostResponseDTO>>> GetHotPosts(
            [FromQuery] PaginationRequestDTO request
        )
        {
            var userAddress = GetUserAddress();
            var user = await _userService.GetUser(userAddress);
            var result = await _recommendationService.GetHotPosts(user, request);

            // return list of all posts
            return Ok(_mapper.Map<IEnumerable<UserPostResponseDTO>>(result));

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
        [BindPost]
        public async Task<ActionResult<IEnumerable<PostLikeResponseDTO>>> GetPostLikes(
            string postNFTAddress,
            [FromQuery] PaginationRequestDTO request
        )
        {
            Post post = (HttpContext.Items["post"] as Post)!;

            var result = await _postService.GetPostLikes(request, post);

            return Ok(_mapper.Map<IEnumerable<PostLikeResponseDTO>>(result));
        }

        [HttpPost("{postNFTAddress}/likes")]
        [BindPost]
        public async Task<ActionResult<BasicPostLikeResponseDTO>> LikePost(string postNFTAddress)
        {
            Post post = (HttpContext.Items["post"] as Post)!;

            var result = await _postService.LikePost(post, GetUserAddress());

            return CreatedAtAction(
                nameof(LikePost),
                new { id = result.Id },
                _mapper.Map<BasicPostLikeResponseDTO>(result)
            );
        }

        [HttpDelete("{postNFTAddress}/likes")]
        [BindPost]
        public async Task<ActionResult<BasicPostLikeResponseDTO>> UnlikePost(string postNFTAddress)
        {
            Post post = (HttpContext.Items["post"] as Post)!;

            await _postService.UnlikePost(post, GetUserAddress());

            return NoContent();
        }
    }
}
