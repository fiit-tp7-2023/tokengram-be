using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Postgres.Entities;

namespace Tokengram.Database.Postgres
{
    public class TokengramDbContext : DbContext
    {
        const int ADDRESS_MAX_LENGTH = 42;

        public TokengramDbContext(DbContextOptions options)
            : base(options) { }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default
        )
        {
            SetTimestamps();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            SetTimestamps();
            return base.SaveChanges();
        }

        private void SetTimestamps()
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.Entity is BaseEntity entity)
                {
                    var now = DateTime.UtcNow;

                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entity.UpdatedAt = now;
                            break;
                        case EntityState.Added:
                            entity.CreatedAt = now;
                            entity.UpdatedAt = now;
                            break;
                    }
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");

                e.Property(x => x.Address).HasColumnName("address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.Username).HasColumnName("username").HasMaxLength(30).IsRequired(false);
                e.Property(x => x.Nonce).HasColumnName("nonce").HasMaxLength(36);

                e.HasKey(x => x.Address);

                e.HasIndex(x => x.Username).IsUnique();
                e.HasIndex(x => x.Nonce).IsUnique();

                e.HasMany(x => x.RefreshTokens).WithOne(x => x.User).HasForeignKey(x => x.UserAddress);
                e.HasMany(x => x.SentChatMessages).WithOne(x => x.Sender).HasForeignKey(x => x.SenderAddress);
                e.HasMany(x => x.ReceivedChatInvitations).WithOne(x => x.User).HasForeignKey(x => x.UserAddress);
                e.HasMany(x => x.ManagedChats).WithOne(x => x.Admin).HasForeignKey(x => x.AdminAddress);
                e.HasMany(x => x.SentChatInvitations).WithOne(x => x.Sender).HasForeignKey(x => x.SenderAddress);
                e.HasMany(x => x.Chats)
                    .WithMany(x => x.Users)
                    .UsingEntity<ChatInvitation>(
                        l => l.HasOne<Chat>().WithMany(e => e.ChatInvitations).HasForeignKey(e => e.ChatId),
                        r => r.HasOne<User>().WithMany(e => e.ReceivedChatInvitations).HasForeignKey(e => e.UserAddress)
                    );
            });

            modelBuilder.Entity<RefreshToken>(e =>
            {
                e.ToTable("refresh_tokens");

                e.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
                e.Property(x => x.UserAddress).HasColumnName("user_address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.Token).HasColumnName("token").HasMaxLength(88);
                e.Property(x => x.ExpiresAt).HasColumnName("expires_at");
                e.Property(x => x.BlackListedAt).HasColumnName("blacklisted_at").IsRequired(false);

                e.HasKey(x => x.Id);

                e.HasIndex(x => x.Token).IsUnique();

                e.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserAddress);
            });

            modelBuilder.Entity<Chat>(e =>
            {
                e.ToTable("chats");

                e.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(30).IsRequired(false);
                e.Property(x => x.AdminAddress).HasColumnName("admin_address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.Type).HasColumnName("type");

                e.HasKey(x => x.Id);

                e.HasOne(x => x.Admin).WithMany(x => x.ManagedChats).HasForeignKey(x => x.AdminAddress);
                e.HasMany(x => x.ChatInvitations).WithOne(x => x.Chat).HasForeignKey(x => x.ChatId);
                e.HasMany(x => x.ChatMessages).WithOne(x => x.Chat).HasForeignKey(x => x.ChatId);
                e.HasMany(x => x.Users)
                    .WithMany(x => x.Chats)
                    .UsingEntity<ChatInvitation>(
                        l =>
                            l.HasOne<User>().WithMany(e => e.ReceivedChatInvitations).HasForeignKey(e => e.UserAddress),
                        r => r.HasOne<Chat>().WithMany(e => e.ChatInvitations).HasForeignKey(e => e.ChatId)
                    );
            });

            modelBuilder.Entity<ChatInvitation>(e =>
            {
                e.ToTable("chat_invitations");

                e.Property(x => x.ChatId).HasColumnName("chat_id");
                e.Property(x => x.UserAddress).HasColumnName("user_address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.SenderAddress)
                    .HasColumnName("sender_address")
                    .HasMaxLength(ADDRESS_MAX_LENGTH)
                    .IsRequired(false);
                e.Property(x => x.JoinedAt).HasColumnName("joined_at");

                e.HasKey(x => new { x.ChatId, x.UserAddress });

                e.HasOne(x => x.User).WithMany(x => x.ReceivedChatInvitations).HasForeignKey(x => x.UserAddress);
                e.HasOne(x => x.Chat).WithMany(x => x.ChatInvitations).HasForeignKey(x => x.ChatId);
                e.HasOne(x => x.Sender).WithMany(x => x.SentChatInvitations).HasForeignKey(x => x.SenderAddress);
            });

            modelBuilder.Entity<ChatMessage>(e =>
            {
                e.ToTable("chat_messages");

                e.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
                e.Property(x => x.Content).HasColumnName("content").HasMaxLength(1000);
                e.Property(x => x.SenderAddress).HasColumnName("sender_address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.ChatId).HasColumnName("chat_id");
                e.Property(x => x.ParentMessageId).HasColumnName("parent_message_id").IsRequired(false);

                e.HasKey(x => x.Id);

                e.HasOne(x => x.Sender).WithMany(x => x.SentChatMessages).HasForeignKey(x => x.SenderAddress);
                e.HasOne(x => x.Chat).WithMany(x => x.ChatMessages).HasForeignKey(x => x.ChatId);
                e.HasOne(x => x.ParentMessage)
                    .WithMany(x => x.MessageReplies)
                    .HasForeignKey(x => x.ParentMessageId)
                    .OnDelete(DeleteBehavior.SetNull);
                e.HasMany(x => x.MessageReplies)
                    .WithOne(x => x.ParentMessage)
                    .HasForeignKey(x => x.ParentMessageId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        public DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<ChatInvitation> ChatInvitations { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }
    }
}
