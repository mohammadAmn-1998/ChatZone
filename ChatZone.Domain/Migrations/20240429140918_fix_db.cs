using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatZone.Domain.Migrations
{
    /// <inheritdoc />
    public partial class fix_db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ReceiverId",
                table: "ChatGroups",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_ReceiverId",
                table: "ChatGroups",
                column: "ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatGroups_Users_ReceiverId",
                table: "ChatGroups",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatGroups_Users_ReceiverId",
                table: "ChatGroups");

            migrationBuilder.DropIndex(
                name: "IX_ChatGroups_ReceiverId",
                table: "ChatGroups");

            migrationBuilder.AlterColumn<long>(
                name: "ReceiverId",
                table: "ChatGroups",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
