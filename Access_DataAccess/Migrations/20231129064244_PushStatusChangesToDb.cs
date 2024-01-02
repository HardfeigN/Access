using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Access_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class PushStatusChangesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusNumber",
                table: "Status");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Status",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Status");

            migrationBuilder.AddColumn<int>(
                name: "StatusNumber",
                table: "Status",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
