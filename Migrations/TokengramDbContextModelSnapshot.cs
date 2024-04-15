﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Tokengram.Database.Tokengram;

#nullable disable

namespace Tokengram.Migrations
{
    [DbContext(typeof(TokengramDbContext))]
    partial class TokengramDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.Chat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<long>("Id"));

                    b.Property<string>("AdminAddress")
                        .IsRequired()
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("admin_address");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Name")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("name");

                    b.Property<byte>("Type")
                        .HasColumnType("smallint")
                        .HasColumnName("type");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("AdminAddress");

                    b.ToTable("chats", (string)null);
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.ChatInvitation", b =>
                {
                    b.Property<long>("ChatId")
                        .HasColumnType("bigint")
                        .HasColumnName("chat_id");

                    b.Property<string>("UserAddress")
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("user_address");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("JoinedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("joined_at");

                    b.Property<string>("SenderAddress")
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("sender_address");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("ChatId", "UserAddress");

                    b.HasIndex("SenderAddress");

                    b.HasIndex("UserAddress");

                    b.ToTable("chat_invitations", (string)null);
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.ChatMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<long>("Id"));

                    b.Property<long>("ChatId")
                        .HasColumnType("bigint")
                        .HasColumnName("chat_id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("content");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long?>("ParentMessageId")
                        .HasColumnType("bigint")
                        .HasColumnName("parent_message_id");

                    b.Property<string>("SenderAddress")
                        .IsRequired()
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("sender_address");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.HasIndex("ParentMessageId");

                    b.HasIndex("SenderAddress");

                    b.ToTable("chat_messages", (string)null);
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.Comment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<long>("Id"));

                    b.Property<string>("CommenterAddress")
                        .IsRequired()
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("commenter_address");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("content");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<long?>("ParentCommentId")
                        .HasColumnType("bigint")
                        .HasColumnName("parent_comment_id");

                    b.Property<string>("PostNFTAddress")
                        .IsRequired()
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("post_nft_address");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("CommenterAddress");

                    b.HasIndex("ParentCommentId");

                    b.HasIndex("PostNFTAddress");

                    b.ToTable("comments", (string)null);
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.CommentLike", b =>
                {
                    b.Property<string>("LikerAddress")
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("liker_address");

                    b.Property<long>("CommentId")
                        .HasColumnType("bigint")
                        .HasColumnName("comment_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("LikerAddress", "CommentId");

                    b.HasIndex("CommentId");

                    b.ToTable("comment_likes", (string)null);
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.Post", b =>
                {
                    b.Property<string>("NFTAddress")
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("nft_address");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("NFTAddress");

                    b.ToTable("posts", (string)null);
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.PostLike", b =>
                {
                    b.Property<string>("LikerAddress")
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("liker_address");

                    b.Property<string>("PostNFTAddress")
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("post_nft_address");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("LikerAddress", "PostNFTAddress");

                    b.HasIndex("PostNFTAddress");

                    b.ToTable("post_likes", (string)null);
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.PostUserSettings", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("description");

                    b.Property<bool>("IsVisible")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_visible");

                    b.Property<string>("PostNFTAddress")
                        .IsRequired()
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("post_nft_address");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UserAddress")
                        .IsRequired()
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("user_address");

                    b.HasKey("Id");

                    b.HasIndex("PostNFTAddress")
                        .IsUnique();

                    b.HasIndex("UserAddress");

                    b.ToTable("post_user_settings", (string)null);
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.RefreshToken", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("BlackListedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("blacklisted_at");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("expires_at");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(88)
                        .HasColumnType("character varying(88)")
                        .HasColumnName("token");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UserAddress")
                        .IsRequired()
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("user_address");

                    b.HasKey("Id");

                    b.HasIndex("Token")
                        .IsUnique();

                    b.HasIndex("UserAddress");

                    b.ToTable("refresh_tokens", (string)null);
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.User", b =>
                {
                    b.Property<string>("Address")
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("address");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Guid>("Nonce")
                        .HasMaxLength(36)
                        .HasColumnType("uuid")
                        .HasColumnName("nonce");

                    b.Property<string>("ProfilePicturePath")
                        .HasColumnType("text")
                        .HasColumnName("profile_picture_path");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<string>("UserVector")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValue("")
                        .HasColumnName("user_vector");

                    b.Property<string>("Username")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("username");

                    b.HasKey("Address");

                    b.HasIndex("Nonce")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.UserFollow", b =>
                {
                    b.Property<string>("FollowerAddress")
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("follower_address");

                    b.Property<string>("FollowedUserAddress")
                        .HasMaxLength(42)
                        .HasColumnType("character varying(42)")
                        .HasColumnName("followed_user_address");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("FollowerAddress", "FollowedUserAddress");

                    b.HasIndex("FollowedUserAddress");

                    b.ToTable("user_follows", (string)null);
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.Chat", b =>
                {
                    b.HasOne("Tokengram.Database.Tokengram.Entities.User", "Admin")
                        .WithMany("ManagedChats")
                        .HasForeignKey("AdminAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.ChatInvitation", b =>
                {
                    b.HasOne("Tokengram.Database.Tokengram.Entities.Chat", "Chat")
                        .WithMany("ChatInvitations")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tokengram.Database.Tokengram.Entities.User", "Sender")
                        .WithMany("SentChatInvitations")
                        .HasForeignKey("SenderAddress")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tokengram.Database.Tokengram.Entities.User", "User")
                        .WithMany("ReceivedChatInvitations")
                        .HasForeignKey("UserAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("Sender");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.ChatMessage", b =>
                {
                    b.HasOne("Tokengram.Database.Tokengram.Entities.Chat", "Chat")
                        .WithMany("ChatMessages")
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tokengram.Database.Tokengram.Entities.ChatMessage", "ParentMessage")
                        .WithMany("MessageReplies")
                        .HasForeignKey("ParentMessageId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tokengram.Database.Tokengram.Entities.User", "Sender")
                        .WithMany("SentChatMessages")
                        .HasForeignKey("SenderAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Chat");

                    b.Navigation("ParentMessage");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.Comment", b =>
                {
                    b.HasOne("Tokengram.Database.Tokengram.Entities.User", "Commenter")
                        .WithMany("Comments")
                        .HasForeignKey("CommenterAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tokengram.Database.Tokengram.Entities.Comment", "ParentComment")
                        .WithMany("CommentReplies")
                        .HasForeignKey("ParentCommentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Tokengram.Database.Tokengram.Entities.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostNFTAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Commenter");

                    b.Navigation("ParentComment");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.CommentLike", b =>
                {
                    b.HasOne("Tokengram.Database.Tokengram.Entities.Comment", "Comment")
                        .WithMany("Likes")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tokengram.Database.Tokengram.Entities.User", "Liker")
                        .WithMany("CommentLikes")
                        .HasForeignKey("LikerAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("Liker");
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.PostLike", b =>
                {
                    b.HasOne("Tokengram.Database.Tokengram.Entities.User", "Liker")
                        .WithMany("PostLikes")
                        .HasForeignKey("LikerAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tokengram.Database.Tokengram.Entities.Post", "Post")
                        .WithMany("Likes")
                        .HasForeignKey("PostNFTAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Liker");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.PostUserSettings", b =>
                {
                    b.HasOne("Tokengram.Database.Tokengram.Entities.Post", "Post")
                        .WithOne("PostUserSettings")
                        .HasForeignKey("Tokengram.Database.Tokengram.Entities.PostUserSettings", "PostNFTAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tokengram.Database.Tokengram.Entities.User", "User")
                        .WithMany("PostUserSettings")
                        .HasForeignKey("UserAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.RefreshToken", b =>
                {
                    b.HasOne("Tokengram.Database.Tokengram.Entities.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.UserFollow", b =>
                {
                    b.HasOne("Tokengram.Database.Tokengram.Entities.User", "FollowedUser")
                        .WithMany("Followers")
                        .HasForeignKey("FollowedUserAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tokengram.Database.Tokengram.Entities.User", "Follower")
                        .WithMany("Followings")
                        .HasForeignKey("FollowerAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FollowedUser");

                    b.Navigation("Follower");
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.Chat", b =>
                {
                    b.Navigation("ChatInvitations");

                    b.Navigation("ChatMessages");
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.ChatMessage", b =>
                {
                    b.Navigation("MessageReplies");
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.Comment", b =>
                {
                    b.Navigation("CommentReplies");

                    b.Navigation("Likes");
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");

                    b.Navigation("PostUserSettings")
                        .IsRequired();
                });

            modelBuilder.Entity("Tokengram.Database.Tokengram.Entities.User", b =>
                {
                    b.Navigation("CommentLikes");

                    b.Navigation("Comments");

                    b.Navigation("Followers");

                    b.Navigation("Followings");

                    b.Navigation("ManagedChats");

                    b.Navigation("PostLikes");

                    b.Navigation("PostUserSettings");

                    b.Navigation("ReceivedChatInvitations");

                    b.Navigation("RefreshTokens");

                    b.Navigation("SentChatInvitations");

                    b.Navigation("SentChatMessages");
                });
#pragma warning restore 612, 618
        }
    }
}
