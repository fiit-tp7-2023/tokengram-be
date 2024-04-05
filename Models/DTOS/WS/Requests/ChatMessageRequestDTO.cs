namespace Tokengram.Models.DTOS.WS.Requests
{
    public class ChatMessageRequestDTO
    {
        public string Content { get; set; } = null!;

        public long? ParentMessageId { get; set; }
    }
}
