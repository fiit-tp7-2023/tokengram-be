namespace Tokengram.Database.Tokengram.Entities
{
    public class ChatMessage : BaseEntity
    {
        public long Id { get; set; }

        public string Content { get; set; } = null!;

        public string SenderAddress { get; set; } = null!;

        public long ChatId { get; set; }

        public long? ParentMessageId { get; set; }

        public User Sender { get; set; } = null!;

        public Chat Chat { get; set; } = null!;

        public ChatMessage? ParentMessage { get; set; }

        public ICollection<ChatMessage> MessageReplies { get; set; } = new List<ChatMessage>();
    }
}
