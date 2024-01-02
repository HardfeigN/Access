using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Access_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addInquiryToOrderToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InquiryToOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderHeaderId = table.Column<int>(type: "int", nullable: true),
                    InquiryHeaderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryToOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryToOrder_InquiryHeader_InquiryHeaderId",
                        column: x => x.InquiryHeaderId,
                        principalTable: "InquiryHeader",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InquiryToOrder_OrderHeader_OrderHeaderId",
                        column: x => x.OrderHeaderId,
                        principalTable: "OrderHeader",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InquiryToOrder_InquiryHeaderId",
                table: "InquiryToOrder",
                column: "InquiryHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryToOrder_OrderHeaderId",
                table: "InquiryToOrder",
                column: "OrderHeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InquiryToOrder");
        }
    }
}
