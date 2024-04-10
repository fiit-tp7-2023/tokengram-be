namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class UserResponseDTO
    {
        public string Address { get; set; } = null!;

        public string? Username { get; set; }

        public string? ProfilePicture { get; set; }

        public bool IsFollower { get; set; } = false;

        public bool IsFollowed { get; set; } = false;

        public long PostCount { get; set; } = 0;

        public long FollowerCount { get; set; } = 0;

        public long FollowingCount { get; set; } = 0;
    }
}
