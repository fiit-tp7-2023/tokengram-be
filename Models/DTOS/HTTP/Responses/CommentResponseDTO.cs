using Tokengram.Models.DTOS.Shared.Responses;

namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class CommentResponseDTO
    {
        public long Id { get; set; }

        public string Content { get; set; } = null!;

        public UserResponseDTO Commenter { get; set; } = null!;

        public string PostNFTAddress { get; set; } = null!;

        public long? ParentCommentId { get; set; }

        public int CommentReplyCount { get; set; } = 0;

        public int LikeCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; }
    }
}
