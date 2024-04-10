using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Tokengram.Migrations
{
    /// <inheritdoc />
    public partial class Refactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_user_follows_users_user_address", table: "user_follows");

            migrationBuilder.DropIndex(name: "IX_post_user_settings_post_nft_address", table: "post_user_settings");

            migrationBuilder.DropPrimaryKey(name: "PK_post_likes", table: "post_likes");

            migrationBuilder.DropIndex(name: "IX_post_likes_liker_address", table: "post_likes");

            migrationBuilder.DropPrimaryKey(name: "PK_comment_likes", table: "comment_likes");

            migrationBuilder.DropIndex(name: "IX_comment_likes_liker_address", table: "comment_likes");

            migrationBuilder.DropColumn(name: "comment_count", table: "posts");

            migrationBuilder.DropColumn(name: "like_count", table: "posts");

            migrationBuilder.DropColumn(name: "id", table: "post_likes");

            migrationBuilder.DropColumn(name: "comment_reply_count", table: "comments");

            migrationBuilder.DropColumn(name: "like_count", table: "comments");

            migrationBuilder.DropColumn(name: "id", table: "comment_likes");

            migrationBuilder.RenameColumn(name: "user_address", table: "user_follows", newName: "follower_address");

            migrationBuilder.AlterColumn<string>(
                name: "user_vector",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_post_likes",
                table: "post_likes",
                columns: new[] { "liker_address", "post_nft_address" }
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_comment_likes",
                table: "comment_likes",
                columns: new[] { "liker_address", "comment_id" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_post_user_settings_post_nft_address",
                table: "post_user_settings",
                column: "post_nft_address",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_user_follows_users_follower_address",
                table: "user_follows",
                column: "follower_address",
                principalTable: "users",
                principalColumn: "address",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_user_follows_users_follower_address", table: "user_follows");

            migrationBuilder.DropIndex(name: "IX_post_user_settings_post_nft_address", table: "post_user_settings");

            migrationBuilder.DropPrimaryKey(name: "PK_post_likes", table: "post_likes");

            migrationBuilder.DropPrimaryKey(name: "PK_comment_likes", table: "comment_likes");

            migrationBuilder.RenameColumn(name: "follower_address", table: "user_follows", newName: "user_address");

            migrationBuilder.AlterColumn<string>(
                name: "user_vector",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: ""
            );

            migrationBuilder.AddColumn<int>(
                name: "comment_count",
                table: "posts",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "like_count",
                table: "posts",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder
                .AddColumn<long>(name: "id", table: "post_likes", type: "bigint", nullable: false, defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddColumn<int>(
                name: "comment_reply_count",
                table: "comments",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "like_count",
                table: "comments",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder
                .AddColumn<long>(name: "id", table: "comment_likes", type: "bigint", nullable: false, defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddPrimaryKey(name: "PK_post_likes", table: "post_likes", column: "id");

            migrationBuilder.AddPrimaryKey(name: "PK_comment_likes", table: "comment_likes", column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_post_user_settings_post_nft_address",
                table: "post_user_settings",
                column: "post_nft_address"
            );

            migrationBuilder.CreateIndex(
                name: "IX_post_likes_liker_address",
                table: "post_likes",
                column: "liker_address"
            );

            migrationBuilder.CreateIndex(
                name: "IX_comment_likes_liker_address",
                table: "comment_likes",
                column: "liker_address"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_user_follows_users_user_address",
                table: "user_follows",
                column: "user_address",
                principalTable: "users",
                principalColumn: "address",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
