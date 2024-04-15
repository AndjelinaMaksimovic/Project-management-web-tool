using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codedberries.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserProjectAddForeignKeyRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserProjects_RoleId",
                table: "UserProjects",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProjects_Roles_RoleId",
                table: "UserProjects",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProjects_Roles_RoleId",
                table: "UserProjects");

            migrationBuilder.DropIndex(
                name: "IX_UserProjects_RoleId",
                table: "UserProjects");
        }
    }
}
