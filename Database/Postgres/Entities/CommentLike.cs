namespace Tokengram.Database.Postgres.Entities
{
    public class CommentLike : BaseEntity
    {
        public long Id { get; set; }

        public string LikerAddress { get; set; } = null!;

        public long CommentId { get; set; }

        public User Liker { get; set; } = null!;

        public Comment Comment { get; set; } = null!;
    }
}
