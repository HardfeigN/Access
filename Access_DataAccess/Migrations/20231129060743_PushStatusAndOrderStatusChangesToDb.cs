using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Access_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class PushStatusAndOrderStatusChangesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeader_OrderStatus_OrderStatusId",
                table: "OrderHeader");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeader_OrderStatusId",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrderStatus");

            migrationBuilder.DropColumn(
                name: "ComplationDate",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "OrderStatusId",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "OrderHeader");

            migrationBuilder.DropColumn(
                name: "ShippingDate",
                table: "OrderHeader");

            migrationBuilder.RenameColumn(
                name: "StatusNumber",
                table: "OrderStatus",
                newName: "StatusId");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "OrderStatus",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "OrderHeaderId",
                table: "OrderStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    StatusNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatus_OrderHeaderId",
                table: "OrderStatus",
                column: "OrderHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatus_StatusId",
                table: "OrderStatus",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStatus_OrderHeader_OrderHeaderId",
                table: "OrderStatus",
                column: "OrderHeaderId",
                principalTable: "OrderHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStatus_Status_StatusId",
                table: "OrderStatus",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderStatus_OrderHeader_OrderHeaderId",
                table: "OrderStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderStatus_Status_StatusId",
                table: "OrderStatus");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropIndex(
                name: "IX_OrderStatus_OrderHeaderId",
                table: "OrderStatus");

            migrationBuilder.DropIndex(
                name: "IX_OrderStatus_StatusId",
                table: "OrderStatus");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "OrderStatus");

            migrationBuilder.DropColumn(
                name: "OrderHeaderId",
                table: "OrderStatus");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "OrderStatus",
                newName: "StatusNumber");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OrderStatus",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ComplationDate",
                table: "OrderHeader",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "OrderHeader",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "OrderStatusId",
                table: "OrderHeader",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "OrderHeader",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippingDate",
                table: "OrderHeader",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeader_OrderStatusId",
                table: "OrderHeader",
                column: "OrderStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeader_OrderStatus_OrderStatusId",
                table: "OrderHeader",
                column: "OrderStatusId",
                principalTable: "OrderStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
