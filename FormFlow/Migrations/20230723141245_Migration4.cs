using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FormFlow.Migrations
{
    /// <inheritdoc />
    public partial class Migration4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseEntries_FormResponses_FormResponseId",
                table: "ResponseEntries");

            migrationBuilder.DropColumn(
                name: "MultipleChoice",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Required",
                table: "Questions");

            migrationBuilder.AlterColumn<int>(
                name: "FormResponseId",
                table: "ResponseEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseEntries_FormResponses_FormResponseId",
                table: "ResponseEntries",
                column: "FormResponseId",
                principalTable: "FormResponses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseEntries_FormResponses_FormResponseId",
                table: "ResponseEntries");

            migrationBuilder.AlterColumn<int>(
                name: "FormResponseId",
                table: "ResponseEntries",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<bool>(
                name: "MultipleChoice",
                table: "Questions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Required",
                table: "Questions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseEntries_FormResponses_FormResponseId",
                table: "ResponseEntries",
                column: "FormResponseId",
                principalTable: "FormResponses",
                principalColumn: "Id");
        }
    }
}
