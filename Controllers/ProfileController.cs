using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.HTTP.Responses;
using Tokengram.Models.DTOS.Shared.Responses;
using Tokengram.Services.Interfaces;

namespace Tokengram.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IProfileService _profileService;

        public ProfileController(IMapper mapper, IProfileService profileService)
        {
            _profileService = profileService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDTO>> ChangeProfileInfo(ChangeProfileInfoRequestDTO request)
        {
            var result = await _profileService.ChangeProfileInfo(GetUserAddress(), request);

            return Ok(_mapper.Map<UserResponseDTO>(result));
        }
    }
}
