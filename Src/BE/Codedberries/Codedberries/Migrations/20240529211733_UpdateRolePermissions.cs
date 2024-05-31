using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codedberries.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRolePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanEditUser",
                table: "Roles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanEditUser",
                table: "Roles");
        }
    }
}
