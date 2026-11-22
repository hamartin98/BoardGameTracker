using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddScoringCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "scoring_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    board_game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    normalized_name = table.Column<string>(type: "text", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_scoring_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_scoring_categories_board_games_board_game_id",
                        column: x => x.board_game_id,
                        principalTable: "board_games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "category_scores",
                columns: table => new
                {
                    player_score_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scoring_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category_scores", x => new { x.player_score_id, x.scoring_category_id });
                    table.ForeignKey(
                        name: "fk_category_scores_player_scores_player_score_id",
                        column: x => x.player_score_id,
                        principalTable: "player_scores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_category_scores_scoring_categories_scoring_category_id",
                        column: x => x.scoring_category_id,
                        principalTable: "scoring_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_category_scores_scoring_category_id",
                table: "category_scores",
                column: "scoring_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_scoring_categories_board_game_id_normalized_name",
                table: "scoring_categories",
                columns: new[] { "board_game_id", "normalized_name" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_scoring_categories_board_game_id_order",
                table: "scoring_categories",
                columns: new[] { "board_game_id", "order" });

            migrationBuilder.CreateIndex(
                name: "ix_scoring_categories_created_at",
                table: "scoring_categories",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_scoring_categories_is_deleted",
                table: "scoring_categories",
                column: "is_deleted",
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "category_scores");

            migrationBuilder.DropTable(
                name: "scoring_categories");
        }
    }
}
