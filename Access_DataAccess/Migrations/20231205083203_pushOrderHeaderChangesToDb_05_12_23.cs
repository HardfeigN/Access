using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Access_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class pushOrderHeaderChangesToDb_05_12_23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerUserId",
                table: "OrderHeader",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_CustomerUserId",
                table: "OrderHeader",
                column: "CustomerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeader_AspNetUsers_CustomerUserId",
                table: "OrderHeader",
                column: "CustomerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeader_AspNetUsers_CustomerUserId",
                table: "OrderHeader");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeader_CustomerUserId",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "CustomerUserId",
                table: "OrderHeader");
        }
    }
}
