using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codedberries.Migrations
{
    /// <inheritdoc />
    public partial class StatusUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Statuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Statuses_ProjectId",
                table: "Statuses",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Statuses_Projects_ProjectId",
                table: "Statuses",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Statuses_Projects_ProjectId",
                table: "Statuses");

            migrationBuilder.DropIndex(
                name: "IX_Statuses_ProjectId",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Statuses");
        }
    }
}
