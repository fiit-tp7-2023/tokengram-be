using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Infrastructure.ActionFilters;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.HTTP.Responses;
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

        public UserController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPut]
        public async Task<ActionResult<BasicUserResponseDTO>> UpdateUser([FromForm] UserUpdateRequest request)
        {
            var result = await _userService.UpdateUser(GetUserAddress(), request);

            return Ok(_mapper.Map<BasicUserResponseDTO>(result));
        }

        [HttpGet("{userAddress}")]
        [BindUser]
        public async Task<ActionResult<UserResponseDTO>> GetUserProfile(string userAddress)
        {
            User user = (HttpContext.Items["user"] as User)!;
            var result = await _userService.GetUserProfile(user, GetUserAddress());

            return Ok(_mapper.Map<UserResponseDTO>(result));
        }

        [HttpGet("{userAddress}/followers")]
        [BindUser]
        public async Task<ActionResult<IEnumerable<FollowerResponseDTO>>> GetUserFollowers(
            string userAddress,
            [FromQuery] PaginationRequestDTO request
        )
        {
            User user = (HttpContext.Items["user"] as User)!;
            var result = await _userService.GetUserFollowers(user, request);

            return Ok(_mapper.Map<IEnumerable<FollowerResponseDTO>>(result));
        }

        [HttpDelete("followers/{userAddress}")]
        [BindUser]
        public async Task<ActionResult> RemoveFollower(string userAddress)
        {
            User deletedFollower = (HttpContext.Items["user"] as User)!;
            await _userService.UnfollowUser(deletedFollower.Address, GetUserAddress());

            return NoContent();
        }

        [HttpGet("{userAddress}/followings")]
        [BindUser]
        public async Task<ActionResult<IEnumerable<FollowingResponseDTO>>> GetUserFollowings(
            string userAddress,
            [FromQuery] PaginationRequestDTO request
        )
        {
            User user = (HttpContext.Items["user"] as User)!;
            var result = await _userService.GetUserFollowings(user, request);

            return Ok(_mapper.Map<IEnumerable<FollowingResponseDTO>>(result));
        }

        [HttpPost("followings/{userAddress}")]
        [BindUser]
        public async Task<ActionResult<FollowingResponseDTO>> FollowUser(string userAddress)
        {
            User targetUser = (HttpContext.Items["user"] as User)!;
            var result = await _userService.FollowUser(GetUserAddress(), targetUser);

            return Created(nameof(FollowUser), _mapper.Map<FollowingResponseDTO>(result));
        }

        [HttpDelete("followings/{userAddress}")]
        [BindUser]
        public async Task<ActionResult> UnfollowUser(string userAddress)
        {
            User targetUser = (HttpContext.Items["user"] as User)!;
            await _userService.UnfollowUser(GetUserAddress(), targetUser);

            return NoContent();
        }
    }
}
