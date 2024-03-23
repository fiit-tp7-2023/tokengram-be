namespace Tokengram.Database.Tokengram.Entities
{
    public class ChatInvitation : BaseEntity
    {
        public string UserAddress { get; set; } = null!;

        public long ChatId { get; set; }

        public string? SenderAddress { get; set; }

        public DateTime? JoinedAt { get; set; }

        public User User { get; set; } = null!;

        public Chat Chat { get; set; } = null!;

        public User? Sender { get; set; }
    }
}
