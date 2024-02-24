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
                        id = table
                            .Column<long>(type: "bigint", nullable: false)
                            .Annotation(
                                "Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                            ),
                        public_address = table.Column<string>(type: "text", nullable: false),
                        nonce = table.Column<Guid>(type: "uuid", nullable: false),
                        refresh_token = table.Column<string>(type: "text", nullable: true),
                        refresh_token_expiry_time = table.Column<DateTime>(
                            type: "timestamp with time zone",
                            nullable: false
                        )
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                }
            );

            migrationBuilder.CreateIndex(name: "IX_users_nonce", table: "users", column: "nonce", unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_public_address",
                table: "users",
                column: "public_address",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "users");
        }
    }
}
