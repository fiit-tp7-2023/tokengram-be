namespace Tokengram.Database.Tokengram.Entities
{
    public class PostUserSettings : BaseEntity
    {
        public long Id { get; set; }

        public string PostNFTAddress { get; set; } = null!;

        public string UserAddress { get; set; } = null!;

        public bool IsVisible { get; set; } = false;

        public string? Description { get; set; }

        public Post Post { get; set; } = null!;

        public User User { get; set; } = null!;
    }
}
