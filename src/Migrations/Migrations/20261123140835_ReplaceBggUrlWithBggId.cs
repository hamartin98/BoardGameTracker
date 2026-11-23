using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceBggUrlWithBggId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "bgg_id",
                table: "board_games",
                type: "integer",
                nullable: true);

            // Extracts the id from URLs like https://boardgamegeek.com/boardgame/169786/scythe
            migrationBuilder.Sql(
                """
                UPDATE board_games
                SET bgg_id = substring(bgg_url from '/boardgame[a-z]*/([0-9]{1,9})(/|$)')::integer
                WHERE bgg_url IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "bgg_url",
                table: "board_games");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bgg_url",
                table: "board_games",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE board_games
                SET bgg_url = 'https://boardgamegeek.com/boardgame/' || bgg_id
                WHERE bgg_id IS NOT NULL;
                """);

            migrationBuilder.DropColumn(
                name: "bgg_id",
                table: "board_games");
        }
    }
}
