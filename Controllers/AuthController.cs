using Microsoft.AspNetCore.Mvc;
using Tokengram.DTOS.Requests;
using Tokengram.DTOS.Responses;
using Tokengram.Services.Interfaces;

namespace Tokengram.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("nonce-message")]
        public async Task<ActionResult<string>> GenerateNonceMessage(NonceRequestDTO request)
        {
            var nonceMessage = await _authService.GenerateNonceMessage(request);

            return Ok(nonceMessage);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokensResponseDTO>> Login(LoginRequestDTO request)
        {
            var result = await _authService.Login(request);

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokensResponseDTO>> Refresh(RefreshTokenRequestDTO request)
        {
            var result = await _authService.RefreshToken(request);

            return Ok(result);
        }
    }
}
