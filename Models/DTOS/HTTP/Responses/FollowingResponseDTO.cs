using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class FollowingResponseDTO
    {
        public string FollowerAddress { get; set; } = null!;
        public BasicUserResponseDTO FollowedUser { get; set; } = null!;
        public DateTime FollowingSince { get; set; }
    }
}
