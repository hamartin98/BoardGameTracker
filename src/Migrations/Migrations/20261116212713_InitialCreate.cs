using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_players", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "board_games",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    min_play_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    max_play_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    min_player_count = table.Column<int>(type: "integer", nullable: false),
                    max_player_count = table.Column<int>(type: "integer", nullable: false),
                    bgg_url = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<byte>(type: "smallint", nullable: false),
                    categories = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    expansion_of_id = table.Column<Guid>(type: "uuid", nullable: true),
                    play_session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_board_games", x => x.id);
                    table.ForeignKey(
                        name: "fk_board_games_board_games_expansion_of_id",
                        column: x => x.expansion_of_id,
                        principalTable: "board_games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "play_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    board_game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    session_type = table.Column<byte>(type: "smallint", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_play_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_play_sessions_board_games_board_game_id",
                        column: x => x.board_game_id,
                        principalTable: "board_games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_scores",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: true),
                    win = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_scores", x => x.id);
                    table.ForeignKey(
                        name: "fk_player_scores_play_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "play_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_player_scores_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_board_games_expansion_of_id",
                table: "board_games",
                column: "expansion_of_id");

            migrationBuilder.CreateIndex(
                name: "ix_board_games_is_deleted",
                table: "board_games",
                column: "is_deleted",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_board_games_play_session_id",
                table: "board_games",
                column: "play_session_id");

            migrationBuilder.CreateIndex(
                name: "ix_play_sessions_board_game_id",
                table: "play_sessions",
                column: "board_game_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_scores_player_id",
                table: "player_scores",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "ix_player_scores_session_id",
                table: "player_scores",
                column: "session_id");

            migrationBuilder.AddForeignKey(
                name: "fk_board_games_play_sessions_play_session_id",
                table: "board_games",
                column: "play_session_id",
                principalTable: "play_sessions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_board_games_play_sessions_play_session_id",
                table: "board_games");

            migrationBuilder.DropTable(
                name: "player_scores");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "play_sessions");

            migrationBuilder.DropTable(
                name: "board_games");
        }
    }
}
