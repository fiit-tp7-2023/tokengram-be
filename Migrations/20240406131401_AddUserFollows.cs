using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tokengram.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFollows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_follows",
                columns: table => new
                {
                    user_address = table.Column<string>(type: "character varying(42)", maxLength: 42, nullable: false),
                    followed_user_address = table.Column<string>(type: "character varying(42)", maxLength: 42, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_follows", x => new { x.user_address, x.followed_user_address });
                    table.ForeignKey(
                        name: "FK_user_follows_users_followed_user_address",
                        column: x => x.followed_user_address,
                        principalTable: "users",
                        principalColumn: "address",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_follows_users_user_address",
                        column: x => x.user_address,
                        principalTable: "users",
                        principalColumn: "address",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_follows_followed_user_address",
                table: "user_follows",
                column: "followed_user_address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_follows");
        }
    }
}
