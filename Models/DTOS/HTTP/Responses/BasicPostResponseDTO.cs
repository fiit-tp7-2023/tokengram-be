namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class BasicPostResponseDTO
    {
        public string NFTAddress { get; set; } = null!;

        public string OwnerAddress { get; set; } = null!;

        public bool IsVisible { get; set; } = false;

        public string? Description { get; set; }

        public int CommentCount { get; set; }

        public int LikeCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
