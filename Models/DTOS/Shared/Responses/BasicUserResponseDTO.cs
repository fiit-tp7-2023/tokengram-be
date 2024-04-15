namespace Tokengram.Models.DTOS.Shared.Responses
{
    public class BasicUserResponseDTO
    {
        public string Address { get; set; } = null!;

        public string? Username { get; set; }

        public string? ProfilePicture { get; set; }
    }
}
