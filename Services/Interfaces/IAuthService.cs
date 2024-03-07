using Tokengram.Models.DTOS.HTTP.Requests;
using Tokengram.Models.DTOS.HTTP.Responses;

namespace Tokengram.Services.Interfaces
{
    public interface IAuthService
    {
        Task<NonceMessageResponseDTO> GenerateNonceMessage(NonceRequestDTO request);
        Task<TokensResponseDTO> Login(LoginRequestDTO request);
        Task<TokensResponseDTO> RefreshToken(RefreshTokenRequestDTO request);
    }
}
