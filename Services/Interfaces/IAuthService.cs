using Tokengram.DTOS.Requests;
using Tokengram.DTOS.Responses;

namespace Tokengram.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateNonceMessage(NonceRequestDTO request);
        Task<TokensResponseDTO> Login(LoginRequestDTO request);
        Task<TokensResponseDTO> RefreshToken(RefreshTokenRequestDTO request);
    }
}
