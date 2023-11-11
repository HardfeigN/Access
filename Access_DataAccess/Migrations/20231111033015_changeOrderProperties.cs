using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Access_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changeOrderProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Order",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CreatedByUserId",
                table: "Order",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AspNetUsers_CreatedByUserId",
                table: "Order",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_CreatedByUserId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_CreatedByUserId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Order");
        }
    }
}
