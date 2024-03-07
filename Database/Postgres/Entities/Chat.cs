using Tokengram.Enums;

namespace Tokengram.Database.Postgres.Entities
{
    public class Chat : BaseEntity
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public string AdminAddress { get; set; } = null!;

        public User Admin { get; set; } = null!;

        public ChatTypeEnum Type { get; set; }

        public ICollection<ChatInvitation> ChatInvitations { get; set; } = new List<ChatInvitation>();

        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
