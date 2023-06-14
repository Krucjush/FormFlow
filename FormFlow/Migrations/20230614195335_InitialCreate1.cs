using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormFlow.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Forms_FormId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_FormId",
                table: "Questions");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Forms",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Forms",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

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
    }
}
