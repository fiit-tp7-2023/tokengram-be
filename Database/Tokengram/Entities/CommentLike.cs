namespace Tokengram.Database.Tokengram.Entities
{
    public class CommentLike : BaseEntity
    {
        public string LikerAddress { get; set; } = null!;

        public long CommentId { get; set; }

        public User Liker { get; set; } = null!;

        public Comment Comment { get; set; } = null!;
    }
}
