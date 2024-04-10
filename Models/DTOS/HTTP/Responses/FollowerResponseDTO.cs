using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class FollowerResponseDTO
    {
        public string FollowedUserAddress { get; set; } = null!;
        public BasicUserResponseDTO Follower { get; set; } = null!;
        public DateTime FollowingSince { get; set; }
    }
}
