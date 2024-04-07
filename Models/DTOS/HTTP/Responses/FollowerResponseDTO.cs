using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class FollowerResponseDTO
    {
        public UserResponseDTO User { get; set; } = null!;
        public DateTime FollowingSince { get; set; }
    }
}
