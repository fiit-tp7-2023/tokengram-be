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
                        principalColumn: "address"
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
                        onDelete: ReferentialAction.SetNull
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
            migrationBuilder.DropTable(name: "chat_invitations");

            migrationBuilder.DropTable(name: "chat_messages");

            migrationBuilder.DropTable(name: "refresh_tokens");

            migrationBuilder.DropTable(name: "chats");

            migrationBuilder.DropTable(name: "users");
        }
    }
}
