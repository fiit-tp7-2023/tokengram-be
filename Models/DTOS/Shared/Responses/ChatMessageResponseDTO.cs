namespace Tokengram.Models.DTOS.Shared.Responses
{
    public class ChatMessageResponseDTO
    {
        public long Id { get; set; }

        public string Content { get; set; } = null!;

        public long ChatId { get; set; }

        public long? ParentMessageId { get; set; }

        public BasicUserResponseDTO Sender { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
