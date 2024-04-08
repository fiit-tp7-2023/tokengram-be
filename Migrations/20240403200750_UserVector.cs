using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tokengram.Migrations
{
    /// <inheritdoc />
    public partial class UserVector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "user_vector",
                table: "users",
                type: "text",
                nullable: true,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "user_vector", table: "users");
        }
    }
}
