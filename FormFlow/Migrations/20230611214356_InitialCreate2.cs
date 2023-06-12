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
                name: "FK_Questions_Forms_FormId1",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_FormId1",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "FormId1",
                table: "Questions");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionId",
                table: "ResponseEntries",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "FormId",
                table: "Questions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Forms",
                type: "INTEGER",
                nullable: true,
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Forms_FormId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_FormId",
                table: "Questions");

            migrationBuilder.AlterColumn<string>(
                name: "QuestionId",
                table: "ResponseEntries",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "FormId",
                table: "Questions",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "FormId1",
                table: "Questions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Forms",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_FormId1",
                table: "Questions",
                column: "FormId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Forms_FormId1",
                table: "Questions",
                column: "FormId1",
                principalTable: "Forms",
                principalColumn: "Id");
        }
    }
}
