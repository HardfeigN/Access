using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Access_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class removeInquiryFromDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeader_AspNetUsers_CreatedByUserId",
                table: "OrderHeader");

            migrationBuilder.DropTable(
                name: "InquiryDetail");

            migrationBuilder.DropTable(
                name: "InquiryToOrder");

            migrationBuilder.DropTable(
                name: "InquiryHeader");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeader_CreatedByUserId",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "OrderHeader");

            migrationBuilder.AddColumn<string>(
                name: "ApproveUserId",
                table: "OrderHeader",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_ApproveUserId",
                table: "OrderHeader",
                column: "ApproveUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeader_AspNetUsers_ApproveUserId",
                table: "OrderHeader",
                column: "ApproveUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeader_AspNetUsers_ApproveUserId",
                table: "OrderHeader");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeader_ApproveUserId",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "ApproveUserId",
                table: "OrderHeader");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "OrderHeader",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "InquiryHeader",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InquiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryHeader", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryHeader_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InquiryDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InquiryHeaderId = table.Column<int>(type: "int", nullable: false),
                    ProductAttributeId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryDetail_InquiryHeader_InquiryHeaderId",
                        column: x => x.InquiryHeaderId,
                        principalTable: "InquiryHeader",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InquiryDetail_ProductAttribute_ProductAttributeId",
                        column: x => x.ProductAttributeId,
                        principalTable: "ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InquiryToOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InquiryHeaderId = table.Column<int>(type: "int", nullable: true),
                    OrderHeaderId = table.Column<int>(type: "int", nullable: true)
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
                name: "IX_OrderHeader_CreatedByUserId",
                table: "OrderHeader",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryDetail_InquiryHeaderId",
                table: "InquiryDetail",
                column: "InquiryHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryDetail_ProductAttributeId",
                table: "InquiryDetail",
                column: "ProductAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryHeader_ApplicationUserId",
                table: "InquiryHeader",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryToOrder_InquiryHeaderId",
                table: "InquiryToOrder",
                column: "InquiryHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryToOrder_OrderHeaderId",
                table: "InquiryToOrder",
                column: "OrderHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeader_AspNetUsers_CreatedByUserId",
                table: "OrderHeader",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
