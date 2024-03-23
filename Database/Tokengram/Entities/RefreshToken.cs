namespace Tokengram.Database.Tokengram.Entities
{
    public class RefreshToken : BaseEntity
    {
        public long Id { get; set; }

        public string UserAddress { get; set; } = null!;

        public string Token { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }

        public DateTime? BlackListedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
