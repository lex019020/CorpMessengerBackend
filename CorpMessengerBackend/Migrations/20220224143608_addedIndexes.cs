using Microsoft.EntityFrameworkCore.Migrations;

namespace CorpMessengerBackend.Migrations
{
    public partial class addedIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserChatLinks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthToken",
                table: "Auths",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatLinks_ChatId",
                table: "UserChatLinks",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatLinks_UserId",
                table: "UserChatLinks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Sent",
                table: "Messages",
                column: "Sent");

            migrationBuilder.CreateIndex(
                name: "IX_Auths_AuthToken",
                table: "Auths",
                column: "AuthToken");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChatLinks_Chats_ChatId",
                table: "UserChatLinks",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserChatLinks_Users_UserId",
                table: "UserChatLinks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChatLinks_Chats_ChatId",
                table: "UserChatLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserChatLinks_Users_UserId",
                table: "UserChatLinks");

            migrationBuilder.DropIndex(
                name: "IX_UserChatLinks_ChatId",
                table: "UserChatLinks");

            migrationBuilder.DropIndex(
                name: "IX_UserChatLinks_UserId",
                table: "UserChatLinks");

            migrationBuilder.DropIndex(
                name: "IX_Messages_Sent",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Auths_AuthToken",
                table: "Auths");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserChatLinks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AuthToken",
                table: "Auths",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
