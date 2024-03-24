namespace Tokengram.Models.DTOS.HTTP.Responses
{
    public class BasicCommentLikeResponseDTO
    {
        public long Id { get; set; }

        public string LikerAddress { get; set; } = null!;

        public long CommentId { get; set; }
    }
}
