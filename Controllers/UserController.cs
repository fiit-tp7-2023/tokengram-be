using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.Shared.Responses;
using Tokengram.Services.Interfaces;

namespace Tokengram.Controllers
{
    [Authorize]
    [ApiController]
    [Route("users")]
    public class UserController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly IChatService _chatService;
        private readonly IFollowerService _followerService;

        public UserController(
            IMapper mapper,
            IUserService userService,
            IPostService postService,
            IChatService chatService,
            IFollowerService followerService
        )
        {
            _mapper = mapper;
            _userService = userService;
            _postService = postService;
            _chatService = chatService;
            _followerService = followerService;
        }

        [HttpPut]
        public async Task<ActionResult<UserResponseDTO>> UpdateUser([FromForm] UserUpdateRequest request)
        {
            var result = await _userService.UpdateUser(GetUserAddress(), request);

            return Ok(_mapper.Map<UserResponseDTO>(result));
        }

        [HttpGet("posts")]
        public async Task<ActionResult<IEnumerable<UserPostResponseDTO>>> GetUserPosts(
            [FromQuery] PaginationRequestDTO request
        )
        {
            var result = await _postService.GetUserPosts(request, GetUserAddress());

            return Ok(_mapper.Map<IEnumerable<UserPostResponseDTO>>(result));
        }

        [HttpGet("chat-profile")]
        public async Task<ActionResult<UserChatProfileResponseDTO>> GetUserChatProfile()
        {
            var result = await _chatService.GetUserChatProfile(GetUserAddress());

            return Ok(_mapper.Map<UserChatProfileResponseDTO>(result));
        }

        [HttpGet("followers")]
        public async Task<ActionResult<IEnumerable<FollowerResponseDTO>>> GetUserFollowers(
            [FromQuery] PaginationRequestDTO request)
        {
            var result = await _followerService.GetUserFollowers(GetUserAddress(), request);
            var dto = result.Select(x => new FollowerResponseDTO { UserAddress = x.UserAddress, FollowingSince = x.CreatedAt });

            return Ok(dto);
        }

        [HttpGet("followings")]
        public async Task<ActionResult<IEnumerable<FollowerResponseDTO>>> GetUserFollowings(
            [FromQuery] PaginationRequestDTO request)
        {
            var result = await _followerService.GetUserFollowings(GetUserAddress(), request);

            return Ok(_mapper.Map<IEnumerable<FollowerResponseDTO>>(result));
        }

        [HttpPost("followings")]
        public async Task<ActionResult<IEnumerable<FollowerResponseDTO>>> FollowUser(FollowRequestDTO request)
        {
            var result = await _followerService.FollowUser(GetUserAddress(), request.UserAddress);

            return Ok(_mapper.Map<FollowerResponseDTO>(result));
        }

        [HttpDelete("followings")]
        public async Task<ActionResult> UnfollowUser(FollowRequestDTO request)
        {
            await _followerService.UnfollowUser(GetUserAddress(), request.UserAddress);

            return NoContent();
        }
    }
}
