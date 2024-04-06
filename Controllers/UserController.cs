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

        public UserController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        [HttpPut]
        public async Task<ActionResult<UserResponseDTO>> UpdateUser([FromForm] UserUpdateRequest request)
        {
            var result = await _userService.UpdateUser(GetUserAddress(), request);

            return Ok(_mapper.Map<UserResponseDTO>(result));
        }
    }
}
