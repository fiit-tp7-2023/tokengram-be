using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class PostResponseDTO
    {
        public long Id { get; set; }

        public UserResponseDTO Owner { get; set; } = null!;

        public NFTResponseDTO NFT { get; set; } = null!;

        public bool IsVisible { get; set; } = false;

        public string? Description { get; set; }

        public int CommentCount { get; set; }

        public int LikeCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
