namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class TokensResponseDTO
    {
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
