namespace Tokengram.Models.DTOS.Shared.Responses
{
    public class UserResponseDTO
    {
        public string Address { get; set; } = null!;

        public string? Username { get; set; }

        public string? ProfilePicture { get; set; }

        public long FollowersCount { get; set; }

        public long FollowingCount { get; set; }
    }
}
