namespace Tokengram.Database.Postgres.Entities
{
    public class User : BaseEntity
    {
        public string Address { get; set; } = null!;

        public string? Username { get; set; }

        public Guid Nonce { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public ICollection<ChatMessage> SentChatMessages { get; set; } = new List<ChatMessage>();

        public ICollection<ChatInvitation> ReceivedChatInvitations { get; set; } = new List<ChatInvitation>();

        public ICollection<Chat> ManagedChats { get; set; } = new List<Chat>();

        public ICollection<ChatInvitation> SentChatInvitations = new List<ChatInvitation>();

        public ICollection<Chat> Chats { get; set; } = new List<Chat>();

        public string GetNonceMessage()
        {
            return $"Tokengram authentication\n\nNonce - {Nonce}\n\nSign this message to authenticate your account. This nonce is unique to this authentication request and helps ensure the security of your account.";
        }
    }
}
