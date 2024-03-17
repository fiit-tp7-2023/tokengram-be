namespace Tokengram.Database.Postgres.Entities
{
    public class PostLike : BaseEntity
    {
        public long Id { get; set; }

        public string LikerAddress { get; set; } = null!;

        public long PostId { get; set; }

        public User Liker { get; set; } = null!;

        public Post Post { get; set; } = null!;
    }
}
