namespace Tokengram.Models.Config
{
    public class JWTOptions
    {
        public string ValidIssuer { get; init; } = null!;

        public string Secret { get; init; } = null!;

        public int TokenValidityInMinutes { get; init; }

        public int RefreshTokenValidityInDays { get; init; }
    }
}
