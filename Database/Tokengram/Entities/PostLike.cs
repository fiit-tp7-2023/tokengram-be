namespace Tokengram.Database.Tokengram.Entities
{
    public class PostLike : BaseEntity
    {
        public string LikerAddress { get; set; } = null!;

        public string PostNFTAddress { get; set; } = null!;

        public User Liker { get; set; } = null!;

        public Post Post { get; set; } = null!;
    }
}
