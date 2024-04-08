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
        private readonly IFollowerService _followerService;

        public UserController(
            IMapper mapper,
            IUserService userService,
            IFollowerService followerService
        )
        {
            _mapper = mapper;
            _userService = userService;
            _followerService = followerService;
        }

        [HttpPut]
        public async Task<ActionResult<UserResponseDTO>> UpdateUser([FromForm] UserUpdateRequest request)
        {
            var result = await _userService.UpdateUser(GetUserAddress(), request);

            return Ok(_mapper.Map<UserResponseDTO>(result));
        }

        [HttpGet("{userAddress}/followers")]
        [BindUser]
        public async Task<ActionResult<IEnumerable<FollowerResponseDTO>>> GetUserFollowers(
            string userAddress,
            [FromQuery] PaginationRequestDTO request)
        {
            User user = (HttpContext.Items["user"] as User)!;
            var result = await _followerService.GetUserFollowers(user, request);
            var dto = result.Select(x => new FollowerResponseDTO {
                User = _mapper.Map<UserResponseDTO>(x.User),
                FollowingSince = x.CreatedAt
            });

            return Ok(dto);
        }

        [HttpDelete("followers/{userAddress}")]
        [BindUser]
        public async Task<ActionResult> RemoveFollower(string userAddress)
        {
            User deletedFollower = (HttpContext.Items["user"] as User)!;
            await _followerService.UnfollowUser(deletedFollower.Address, GetUserAddress());

            return NoContent();
        }

        [HttpGet("{userAddress}/followings")]
        [BindUser]
        public async Task<ActionResult<IEnumerable<FollowerResponseDTO>>> GetUserFollowings(
            string userAddress,
            [FromQuery] PaginationRequestDTO request)
        {
            User user = (HttpContext.Items["user"] as User)!;
            var result = await _followerService.GetUserFollowings(user, request);

            return Ok(_mapper.Map<IEnumerable<FollowerResponseDTO>>(result));
        }

        [HttpPost("followings/{userAddress}")]
        [BindUser]
        public async Task<ActionResult<IEnumerable<FollowerResponseDTO>>> FollowUser(string userAddress)
        {
            User targetUser = (HttpContext.Items["user"] as User)!;
            var result = await _followerService.FollowUser(GetUserAddress(), targetUser);

            return Ok(_mapper.Map<FollowerResponseDTO>(result));
        }

        [HttpDelete("followings/{userAddress}")]
        [BindUser]
        public async Task<ActionResult> UnfollowUser(string userAddress)
        {
            User targetUser = (HttpContext.Items["user"] as User)!;
            await _followerService.UnfollowUser(GetUserAddress(), targetUser);

            return NoContent();
        }
    }
}
