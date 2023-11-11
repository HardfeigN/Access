using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Access_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changeAttributies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttributeTypeId",
                table: "ProductAttribute",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AttributeValue",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7);

            migrationBuilder.CreateTable(
                name: "AttributeType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_AttributeTypeId",
                table: "ProductAttribute",
                column: "AttributeTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttribute_AttributeType_AttributeTypeId",
                table: "ProductAttribute",
                column: "AttributeTypeId",
                principalTable: "AttributeType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttribute_AttributeType_AttributeTypeId",
                table: "ProductAttribute");

            migrationBuilder.DropTable(
                name: "AttributeType");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttribute_AttributeTypeId",
                table: "ProductAttribute");

            migrationBuilder.DropColumn(
                name: "AttributeTypeId",
                table: "ProductAttribute");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AttributeValue",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);
        }
    }
}
