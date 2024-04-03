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
    public partial class RecommendationController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IRecommendationService _recommendationService;

        private readonly IUserService _userService;

        public RecommendationController(
            IMapper mapper,
            IRecommendationService recommendationService,
            IUserService userService
        )
        {
            _mapper = mapper;
            _recommendationService = recommendationService;
            _userService = userService;
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
        }
    }
}
