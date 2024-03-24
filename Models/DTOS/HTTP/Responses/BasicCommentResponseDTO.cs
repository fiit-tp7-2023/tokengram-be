namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class BasicCommentResponseDTO
    {
        public long Id { get; set; }

        public string Content { get; set; } = null!;

        public string CommenterAddress { get; set; } = null!;

        public string PostNFTAddress { get; set; } = null!;

        public long? ParentCommentId { get; set; }

        public int CommentReplyCount { get; set; } = 0;

        public int LikeCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; }
    }
}
