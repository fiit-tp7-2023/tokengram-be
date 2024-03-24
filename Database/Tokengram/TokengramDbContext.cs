using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Constants;

namespace Tokengram.Database.Tokengram
{
    public class TokengramDbContext : DbContext
    {
        const int ADDRESS_MAX_LENGTH = 42;
        const int USERNAME_MAX_LENGTH = ProfileSettings.MAX_USERNAME_LENGTH;

        public TokengramDbContext(DbContextOptions<TokengramDbContext> options)
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
                e.Property(x => x.Username).HasColumnName("username").HasMaxLength(USERNAME_MAX_LENGTH).IsRequired(false);
                e.Property(x => x.Nonce).HasColumnName("nonce").HasMaxLength(36);
                e.Property(x => x.ProfilePicturePath).HasColumnName("profile_picture_path").IsRequired(false);

                e.HasKey(x => x.Address);

                e.HasIndex(x => x.Username).IsUnique();
                e.HasIndex(x => x.Nonce).IsUnique();

                e.HasMany(x => x.RefreshTokens)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.SentChatMessages)
                    .WithOne(x => x.Sender)
                    .HasForeignKey(x => x.SenderAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.ReceivedChatInvitations)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.ManagedChats)
                    .WithOne(x => x.Admin)
                    .HasForeignKey(x => x.AdminAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.SentChatInvitations)
                    .WithOne(x => x.Sender)
                    .HasForeignKey(x => x.SenderAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.Chats)
                    .WithMany(x => x.Users)
                    .UsingEntity<ChatInvitation>(
                        l =>
                            l.HasOne<Chat>()
                                .WithMany(e => e.ChatInvitations)
                                .HasForeignKey(e => e.ChatId)
                                .OnDelete(DeleteBehavior.Cascade),
                        r =>
                            r.HasOne<User>()
                                .WithMany(e => e.ReceivedChatInvitations)
                                .HasForeignKey(e => e.UserAddress)
                                .OnDelete(DeleteBehavior.Cascade)
                    );
                e.HasMany(x => x.Comments)
                    .WithOne(x => x.Commenter)
                    .HasForeignKey(x => x.CommenterAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.CommentLikes)
                    .WithOne(x => x.Liker)
                    .HasForeignKey(x => x.LikerAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.PostLikes)
                    .WithOne(x => x.Liker)
                    .HasForeignKey(x => x.LikerAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.PostUserSettings)
                    .WithOne(x => x.User)
                    .HasForeignKey(x => x.UserAddress)
                    .OnDelete(DeleteBehavior.Cascade);
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

                e.HasOne(x => x.User)
                    .WithMany(x => x.RefreshTokens)
                    .HasForeignKey(x => x.UserAddress)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Chat>(e =>
            {
                e.ToTable("chats");

                e.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(30).IsRequired(false);
                e.Property(x => x.AdminAddress).HasColumnName("admin_address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.Type).HasColumnName("type");

                e.HasKey(x => x.Id);

                e.HasOne(x => x.Admin)
                    .WithMany(x => x.ManagedChats)
                    .HasForeignKey(x => x.AdminAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.ChatInvitations)
                    .WithOne(x => x.Chat)
                    .HasForeignKey(x => x.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.ChatMessages)
                    .WithOne(x => x.Chat)
                    .HasForeignKey(x => x.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.Users)
                    .WithMany(x => x.Chats)
                    .UsingEntity<ChatInvitation>(
                        l =>
                            l.HasOne<User>()
                                .WithMany(e => e.ReceivedChatInvitations)
                                .HasForeignKey(e => e.UserAddress)
                                .OnDelete(DeleteBehavior.Cascade),
                        r =>
                            r.HasOne<Chat>()
                                .WithMany(e => e.ChatInvitations)
                                .HasForeignKey(e => e.ChatId)
                                .OnDelete(DeleteBehavior.Cascade)
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

                e.HasOne(x => x.Sender)
                    .WithMany(x => x.SentChatMessages)
                    .HasForeignKey(x => x.SenderAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Chat)
                    .WithMany(x => x.ChatMessages)
                    .HasForeignKey(x => x.ChatId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.ParentMessage)
                    .WithMany(x => x.MessageReplies)
                    .HasForeignKey(x => x.ParentMessageId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.MessageReplies)
                    .WithOne(x => x.ParentMessage)
                    .HasForeignKey(x => x.ParentMessageId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Post>(e =>
            {
                e.ToTable("posts");

                e.Property(x => x.NFTAddress).HasColumnName("nft_address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.CommentCount).HasColumnName("comment_count").HasDefaultValue(0);
                e.Property(x => x.LikeCount).HasColumnName("like_count").HasDefaultValue(0);

                e.HasKey(x => x.NFTAddress);

                e.HasMany(x => x.Comments)
                    .WithOne(x => x.Post)
                    .HasForeignKey(x => x.PostNFTAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.Likes)
                    .WithOne(x => x.Post)
                    .HasForeignKey(x => x.PostNFTAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.PostUserSettings)
                    .WithOne(x => x.Post)
                    .HasForeignKey(x => x.PostNFTAddress)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Comment>(e =>
            {
                e.ToTable("comments");

                e.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
                e.Property(x => x.CommenterAddress).HasColumnName("commenter_address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.PostNFTAddress).HasColumnName("post_nft_address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.ParentCommentId).HasColumnName("parent_comment_id");
                e.Property(x => x.CommentReplyCount).HasColumnName("comment_reply_count").HasDefaultValue(0);
                e.Property(x => x.LikeCount).HasColumnName("like_count").HasDefaultValue(0);
                e.Property(x => x.Content).HasColumnName("content").HasMaxLength(500);

                e.HasKey(x => x.Id);

                e.HasOne(x => x.Commenter)
                    .WithMany(x => x.Comments)
                    .HasForeignKey(x => x.CommenterAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Post)
                    .WithMany(x => x.Comments)
                    .HasForeignKey(x => x.PostNFTAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.ParentComment)
                    .WithMany(x => x.CommentReplies)
                    .HasForeignKey(x => x.ParentCommentId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.Likes)
                    .WithOne(x => x.Comment)
                    .HasForeignKey(x => x.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.CommentReplies)
                    .WithOne(x => x.ParentComment)
                    .HasForeignKey(x => x.ParentCommentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PostLike>(e =>
            {
                e.ToTable("post_likes");

                e.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
                e.Property(x => x.LikerAddress).HasColumnName("liker_address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.PostNFTAddress).HasColumnName("post_nft_address").HasMaxLength(ADDRESS_MAX_LENGTH);

                e.HasKey(x => x.Id);

                e.HasOne(x => x.Liker)
                    .WithMany(x => x.PostLikes)
                    .HasForeignKey(x => x.LikerAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Post)
                    .WithMany(x => x.Likes)
                    .HasForeignKey(x => x.PostNFTAddress)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CommentLike>(e =>
            {
                e.ToTable("comment_likes");

                e.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
                e.Property(x => x.LikerAddress).HasColumnName("liker_address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.CommentId).HasColumnName("comment_id");

                e.HasKey(x => x.Id);

                e.HasOne(x => x.Liker)
                    .WithMany(x => x.CommentLikes)
                    .HasForeignKey(x => x.LikerAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Comment)
                    .WithMany(x => x.Likes)
                    .HasForeignKey(x => x.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PostUserSettings>(e =>
            {
                e.ToTable("post_user_settings");

                e.Property(x => x.Id).HasColumnName("id").UseIdentityAlwaysColumn();
                e.Property(x => x.PostNFTAddress).HasColumnName("post_nft_address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.UserAddress).HasColumnName("user_address").HasMaxLength(ADDRESS_MAX_LENGTH);
                e.Property(x => x.IsVisible).HasColumnName("is_visible").HasDefaultValue(false);
                e.Property(x => x.Description).HasColumnName("description").HasMaxLength(500).IsRequired(false);

                e.HasKey(x => x.Id);

                e.HasOne(x => x.Post)
                    .WithMany(x => x.PostUserSettings)
                    .HasForeignKey(x => x.PostNFTAddress)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.User)
                    .WithMany(x => x.PostUserSettings)
                    .HasForeignKey(x => x.UserAddress)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Chat> Chats { get; set; }

        public DbSet<ChatInvitation> ChatInvitations { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<CommentLike> CommentLikes { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<PostLike> PostLikes { get; set; }

        public DbSet<PostUserSettings> PostUserSettings { get; set; }
    }
}
