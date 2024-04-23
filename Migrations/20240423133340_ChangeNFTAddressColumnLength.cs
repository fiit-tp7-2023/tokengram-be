using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tokengram.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNFTAddressColumnLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "nft_address",
                table: "posts",
                type: "character varying(62)",
                maxLength: 62,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(42)",
                oldMaxLength: 42
            );

            migrationBuilder.AlterColumn<string>(
                name: "post_nft_address",
                table: "post_user_settings",
                type: "character varying(62)",
                maxLength: 62,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(42)",
                oldMaxLength: 42
            );

            migrationBuilder.AlterColumn<string>(
                name: "post_nft_address",
                table: "post_likes",
                type: "character varying(62)",
                maxLength: 62,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(42)",
                oldMaxLength: 42
            );

            migrationBuilder.AlterColumn<string>(
                name: "post_nft_address",
                table: "comments",
                type: "character varying(62)",
                maxLength: 62,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(42)",
                oldMaxLength: 42
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "nft_address",
                table: "posts",
                type: "character varying(42)",
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(62)",
                oldMaxLength: 62
            );

            migrationBuilder.AlterColumn<string>(
                name: "post_nft_address",
                table: "post_user_settings",
                type: "character varying(42)",
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(62)",
                oldMaxLength: 62
            );

            migrationBuilder.AlterColumn<string>(
                name: "post_nft_address",
                table: "post_likes",
                type: "character varying(42)",
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(62)",
                oldMaxLength: 62
            );

            migrationBuilder.AlterColumn<string>(
                name: "post_nft_address",
                table: "comments",
                type: "character varying(42)",
                maxLength: 42,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(62)",
                oldMaxLength: 62
            );
        }
    }
}
