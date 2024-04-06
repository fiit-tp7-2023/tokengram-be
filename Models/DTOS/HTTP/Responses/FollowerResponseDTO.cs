namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class FollowerResponseDTO
    {
        public string UserAddress { get; set; } = null!;
        public DateTime FollowingSince { get; set; }
    }
}
