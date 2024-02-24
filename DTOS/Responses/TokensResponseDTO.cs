namespace Tokengram.DTOS.Responses
{
    public class TokensResponseDTO
    {
        public string AccessToken { get; set; } = null!;

        public string RefreshToken { get; set; } = null!;
    }
}
