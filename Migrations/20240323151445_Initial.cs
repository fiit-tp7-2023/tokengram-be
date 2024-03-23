using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tokengram.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "posts",
                columns: table =>
                    new
                    {
                        nft_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: false
                        ),
                        comment_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                        like_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posts", x => x.nft_address);
                }
            );

            migrationBuilder.CreateTable(
                name: "users",
                columns: table =>
                    new
                    {
                        address = table.Column<string>(type: "character varying(42)", maxLength: 42, nullable: false),
                        username = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                        nonce = table.Column<Guid>(type: "uuid", maxLength: 36, nullable: false),
                        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.address);
                }
            );

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityAlwaysColumn
                            ),
                        content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                        commenter_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: false
                        ),
                        post_nft_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: false
                        ),
                        parent_comment_id = table.Column<long>(type: "bigint", nullable: true),
                        comment_reply_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                        like_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.id);
                    table.ForeignKey(
                        name: "FK_comments_comments_parent_comment_id",
                        column: x => x.parent_comment_id,
                        principalTable: "comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_comments_posts_post_nft_address",
                        column: x => x.post_nft_address,
                        principalTable: "posts",
                        principalColumn: "nft_address",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_comments_users_commenter_address",
                        column: x => x.commenter_address,
                        principalTable: "users",
                        principalColumn: "address",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "chats",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityAlwaysColumn
                            ),
                        name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                        admin_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: false
                        ),
                        type = table.Column<byte>(type: "smallint", nullable: false),
                        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chats", x => x.id);
                    table.ForeignKey(
                        name: "FK_chats_users_admin_address",
                        column: x => x.admin_address,
                        principalTable: "users",
                        principalColumn: "address",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "post_likes",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityAlwaysColumn
                            ),
                        liker_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: false
                        ),
                        post_nft_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: false
                        ),
                        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_likes", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_likes_posts_post_nft_address",
                        column: x => x.post_nft_address,
                        principalTable: "posts",
                        principalColumn: "nft_address",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_post_likes_users_liker_address",
                        column: x => x.liker_address,
                        principalTable: "users",
                        principalColumn: "address",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "post_user_settings",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityAlwaysColumn
                            ),
                        post_nft_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: false
                        ),
                        user_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: false
                        ),
                        is_visible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                        description = table.Column<string>(
                            type: "character varying(500)",
                            maxLength: 500,
                            nullable: true
                        ),
                        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_post_user_settings", x => x.id);
                    table.ForeignKey(
                        name: "FK_post_user_settings_posts_post_nft_address",
                        column: x => x.post_nft_address,
                        principalTable: "posts",
                        principalColumn: "nft_address",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_post_user_settings_users_user_address",
                        column: x => x.user_address,
                        principalTable: "users",
                        principalColumn: "address",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityAlwaysColumn
                            ),
                        user_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: false
                        ),
                        token = table.Column<string>(type: "character varying(88)", maxLength: 88, nullable: false),
                        expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        blacklisted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_user_address",
                        column: x => x.user_address,
                        principalTable: "users",
                        principalColumn: "address",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "comment_likes",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityAlwaysColumn
                            ),
                        liker_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: false
                        ),
                        comment_id = table.Column<long>(type: "bigint", nullable: false),
                        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment_likes", x => x.id);
                    table.ForeignKey(
                        name: "FK_comment_likes_comments_comment_id",
                        column: x => x.comment_id,
                        principalTable: "comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_comment_likes_users_liker_address",
                        column: x => x.liker_address,
                        principalTable: "users",
                        principalColumn: "address",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "chat_invitations",
                columns: table =>
                    new
                    {
                        user_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: false
                        ),
                        chat_id = table.Column<long>(type: "bigint", nullable: false),
                        sender_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: true
                        ),
                        joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_invitations", x => new { x.chat_id, x.user_address });
                    table.ForeignKey(
                        name: "FK_chat_invitations_chats_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_chat_invitations_users_sender_address",
                        column: x => x.sender_address,
                        principalTable: "users",
                        principalColumn: "address",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_chat_invitations_users_user_address",
                        column: x => x.user_address,
                        principalTable: "users",
                        principalColumn: "address",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "chat_messages",
                columns: table =>
                    new
                    {
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityAlwaysColumn
                            ),
                        content = table.Column<string>(
                            type: "character varying(1000)",
                            maxLength: 1000,
                            nullable: false
                        ),
                        sender_address = table.Column<string>(
                            type: "character varying(42)",
                            maxLength: 42,
                            nullable: false
                        ),
                        chat_id = table.Column<long>(type: "bigint", nullable: false),
                        parent_message_id = table.Column<long>(type: "bigint", nullable: true),
                        created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chat_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_chat_messages_chat_messages_parent_message_id",
                        column: x => x.parent_message_id,
                        principalTable: "chat_messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_chat_messages_chats_chat_id",
                        column: x => x.chat_id,
                        principalTable: "chats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_chat_messages_users_sender_address",
                        column: x => x.sender_address,
                        principalTable: "users",
                        principalColumn: "address",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_comment_likes_comment_id",
                table: "comment_likes",
                column: "comment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_comment_likes_liker_address",
                table: "comment_likes",
                column: "liker_address"
            );

            migrationBuilder.CreateIndex(
                name: "IX_comments_commenter_address",
                table: "comments",
                column: "commenter_address"
            );

            migrationBuilder.CreateIndex(
                name: "IX_comments_parent_comment_id",
                table: "comments",
                column: "parent_comment_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_comments_post_nft_address",
                table: "comments",
                column: "post_nft_address"
            );

            migrationBuilder.CreateIndex(
                name: "IX_chat_invitations_sender_address",
                table: "chat_invitations",
                column: "sender_address"
            );

            migrationBuilder.CreateIndex(
                name: "IX_chat_invitations_user_address",
                table: "chat_invitations",
                column: "user_address"
            );

            migrationBuilder.CreateIndex(name: "IX_chat_messages_chat_id", table: "chat_messages", column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_parent_message_id",
                table: "chat_messages",
                column: "parent_message_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_chat_messages_sender_address",
                table: "chat_messages",
                column: "sender_address"
            );

            migrationBuilder.CreateIndex(name: "IX_chats_admin_address", table: "chats", column: "admin_address");

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_liker_address",
                table: "post_likes",
                column: "liker_address"
            );

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_post_nft_address",
                table: "post_likes",
                column: "post_nft_address"
            );

            migrationBuilder.CreateIndex(
                name: "IX_post_user_settings_post_nft_address",
                table: "post_user_settings",
                column: "post_nft_address"
            );

            migrationBuilder.CreateIndex(
                name: "IX_post_user_settings_user_address",
                table: "post_user_settings",
                column: "user_address"
            );

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_token",
                table: "refresh_tokens",
                column: "token",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_address",
                table: "refresh_tokens",
                column: "user_address"
            );

            migrationBuilder.CreateIndex(name: "IX_users_nonce", table: "users", column: "nonce", unique: true);

            migrationBuilder.CreateIndex(name: "IX_users_username", table: "users", column: "username", unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "comment_likes");

            migrationBuilder.DropTable(name: "chat_invitations");

            migrationBuilder.DropTable(name: "chat_messages");

            migrationBuilder.DropTable(name: "post_likes");

            migrationBuilder.DropTable(name: "post_user_settings");

            migrationBuilder.DropTable(name: "refresh_tokens");

            migrationBuilder.DropTable(name: "comments");

            migrationBuilder.DropTable(name: "chats");

            migrationBuilder.DropTable(name: "posts");

            migrationBuilder.DropTable(name: "users");
        }
    }
}
