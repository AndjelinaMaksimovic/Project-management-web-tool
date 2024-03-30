using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codedberries.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStatusNameUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Statuses_Name",
                table: "Statuses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Statuses_Name",
                table: "Statuses",
                column: "Name",
                unique: true);
        }
    }
}
