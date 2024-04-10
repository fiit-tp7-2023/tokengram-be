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
        private readonly IUserService _userService;

        public PostController(
            IMapper mapper,
            IPostService postService,
            ICommentService commentService,
            IUserService userService
        )
        {
            _mapper = mapper;
            _postService = postService;
            _userService = userService;
            _commentService = commentService;
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<PostResponseDTO>>> GetCurrentUserPosts(
            [FromQuery] GetUserPostsRequestDTO request
        )
        {
            var result = await _postService.GetUserPosts(
                new PaginationRequestDTO { PageNumber = request.PageNumber, PageSize = request.PageSize },
                GetUserAddress(),
                request.IsVisible
            );

            return Ok(_mapper.Map<IEnumerable<PostResponseDTO>>(result));
        }

        [HttpGet("user/{userAddress}")]
        [BindUser]
        public async Task<ActionResult<IEnumerable<PostResponseDTO>>> GetUserPosts(
            string userAddress,
            [FromQuery] PaginationRequestDTO request
        )
        {
            User user = (HttpContext.Items["user"] as User)!;
            var result = await _postService.GetUserPosts(request, user.Address);

            return Ok(_mapper.Map<IEnumerable<PostResponseDTO>>(result));
        }

        [HttpGet("hot")]
        public async Task<ActionResult<IEnumerable<PostResponseDTO>>> GetHotPosts(
            [FromQuery] PaginationRequestDTO request
        )
        {
            var user = await _userService.GetUser(GetUserAddress());
            var result = await _postService.GetHotPosts(user, request);

            return Ok(_mapper.Map<IEnumerable<PostResponseDTO>>(result));
        }

        [HttpPut("{postNFTAddress}/settings")]
        public async Task<ActionResult<PostResponseDTO>> UpdatePostUserSettings(
            string postNFTAddress,
            PostUserSettingsRequestDTO request
        )
        {
            var result = await _postService.UpdatePostUserSettings(request, postNFTAddress, GetUserAddress());

            return Ok(_mapper.Map<PostResponseDTO>(result));
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
        public async Task<ActionResult<PostLikeResponseDTO>> LikePost(string postNFTAddress)
        {
            Post post = (HttpContext.Items["post"] as Post)!;

            var result = await _postService.LikePost(post, GetUserAddress());

            return Created(nameof(LikePost), _mapper.Map<PostLikeResponseDTO>(result));
        }

        [HttpDelete("{postNFTAddress}/likes")]
        [BindPost]
        public async Task<ActionResult> UnlikePost(string postNFTAddress)
        {
            Post post = (HttpContext.Items["post"] as Post)!;

            await _postService.UnlikePost(post, GetUserAddress());

            return NoContent();
        }
    }
}
