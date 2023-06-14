using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormFlow.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Questions_QuestionId",
                table: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_Forms_QuestionId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "Forms");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_FormId",
                table: "Questions",
                column: "FormId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Forms_FormId",
                table: "Questions",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Forms_FormId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_FormId",
                table: "Questions");

            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "Forms",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_QuestionId",
                table: "Forms",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Questions_QuestionId",
                table: "Forms",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");
        }
    }
}
