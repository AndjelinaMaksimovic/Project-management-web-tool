using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codedberries.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStatusAddOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Statuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Statuses");
        }
    }
}
