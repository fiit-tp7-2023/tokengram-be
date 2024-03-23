namespace Tokengram.Models.DTOS.HTTP.Requests
{
    public class CommentRequestDTO
    {
        public string Content { get; set; } = null!;

        public long? ParentCommentId { get; set; }
    }
}
