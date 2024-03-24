namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class OwnedPostWithUserContextResponseDTO
    {
        public long Id { get; set; }

        public string OwnerAddress { get; set; } = null!;

        public NFTResponseDTO NFT { get; set; } = null!;

        public string? Description { get; set; }

        public int CommentCount { get; set; } = 0;

        public int LikeCount { get; set; } = 0;

        public bool IsVisible { get; set; } = false;

        public bool IsLiked { get; set; } = false;

        public DateTime CreatedAt { get; set; }
    }
}
