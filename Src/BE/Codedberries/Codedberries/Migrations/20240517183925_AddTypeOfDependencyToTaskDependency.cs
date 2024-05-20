using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codedberries.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeOfDependencyToTaskDependency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeOfDependencyId",
                table: "TaskDependency",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TaskDependency_TypeOfDependencyId",
                table: "TaskDependency",
                column: "TypeOfDependencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskDependency_TypesOfTaskDependency_TypeOfDependencyId",
                table: "TaskDependency",
                column: "TypeOfDependencyId",
                principalTable: "TypesOfTaskDependency",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskDependency_TypesOfTaskDependency_TypeOfDependencyId",
                table: "TaskDependency");

            migrationBuilder.DropIndex(
                name: "IX_TaskDependency_TypeOfDependencyId",
                table: "TaskDependency");

            migrationBuilder.DropColumn(
                name: "TypeOfDependencyId",
                table: "TaskDependency");
        }
    }
}
