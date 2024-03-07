namespace Tokengram.Models.Hubs
{
    public class ChatGroup
    {
        public long ChatId { get; set; }
        public List<ConnectedUser> ConnectedUsers { get; set; } = new List<ConnectedUser>();
    }
}
