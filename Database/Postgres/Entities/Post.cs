namespace Tokengram.Database.Postgres.Entities
{
    public class Post : BaseEntity
    {
        public long Id { get; set; }

        public string NftAddress { get; set; } = null!;

        public string? OwnerAddress { get; set; }

        public int LikeCount { get; set; } = 0;

        public bool IsVisible { get; set; } = false;

        public User? Owner { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    }
}
