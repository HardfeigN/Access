using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Access_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class pushInquiryAndOrderChangesToDB_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetail_Product_ProductId",
                table: "InquiryDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_Product_ProductId",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_InquiryDetail_ProductId",
                table: "InquiryDetail");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderDetail",
                newName: "ProductAttributeId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetail_ProductId",
                table: "OrderDetail",
                newName: "IX_OrderDetail_ProductAttributeId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "InquiryDetail",
                newName: "Quantity");

            migrationBuilder.AddColumn<string>(
                name: "FullAddress",
                table: "InquiryHeader",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProductAttributeId",
                table: "InquiryDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InquiryDetail_ProductAttributeId",
                table: "InquiryDetail",
                column: "ProductAttributeId");

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetail_ProductAttribute_ProductAttributeId",
                table: "InquiryDetail",
                column: "ProductAttributeId",
                principalTable: "ProductAttribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_ProductAttribute_ProductAttributeId",
                table: "OrderDetail",
                column: "ProductAttributeId",
                principalTable: "ProductAttribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InquiryDetail_ProductAttribute_ProductAttributeId",
                table: "InquiryDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetail_ProductAttribute_ProductAttributeId",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_InquiryDetail_ProductAttributeId",
                table: "InquiryDetail");

            migrationBuilder.DropColumn(
                name: "FullAddress",
                table: "InquiryHeader");

            migrationBuilder.DropColumn(
                name: "ProductAttributeId",
                table: "InquiryDetail");

            migrationBuilder.RenameColumn(
                name: "ProductAttributeId",
                table: "OrderDetail",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetail_ProductAttributeId",
                table: "OrderDetail",
                newName: "IX_OrderDetail_ProductId");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "InquiryDetail",
                newName: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InquiryDetail_ProductId",
                table: "InquiryDetail",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_InquiryDetail_Product_ProductId",
                table: "InquiryDetail",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetail_Product_ProductId",
                table: "OrderDetail",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
