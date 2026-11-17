using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "normalized_name",
                table: "board_games",
                type: "text",
                nullable: false,
                defaultValue: "");

            // Update existing data before adding index
            migrationBuilder.Sql("UPDATE board_games SET normalized_name = LOWER(name);");

            migrationBuilder.CreateIndex(
                name: "ix_board_games_created_at",
                table: "board_games",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_board_games_normalized_name",
                table: "board_games",
                column: "normalized_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_board_games_created_at",
                table: "board_games");

            migrationBuilder.DropIndex(
                name: "ix_board_games_normalized_name",
                table: "board_games");

            migrationBuilder.DropColumn(
                name: "normalized_name",
                table: "board_games");
        }
    }
}
