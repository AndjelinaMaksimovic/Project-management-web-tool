using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codedberries.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskV6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archieved",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archieved",
                table: "Tasks");
        }
    }
}
