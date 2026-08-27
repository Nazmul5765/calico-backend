using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lofi_backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedPlaylistUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_Users_UserDataId",
                table: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_UserDataId",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "UserDataId",
                table: "Playlists");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserDataId",
                table: "Playlists",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_UserDataId",
                table: "Playlists",
                column: "UserDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_Users_UserDataId",
                table: "Playlists",
                column: "UserDataId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
