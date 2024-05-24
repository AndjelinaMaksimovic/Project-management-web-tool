using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codedberries.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskCommentAddCommentDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CommentDate",
                table: "TaskComments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentDate",
                table: "TaskComments");
        }
    }
}
