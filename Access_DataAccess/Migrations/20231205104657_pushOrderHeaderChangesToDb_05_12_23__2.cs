using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Access_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class pushOrderHeaderChangesToDb_05_12_23__2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "OrderCost",
                table: "OrderHeader",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderCost",
                table: "OrderHeader");
        }
    }
}
