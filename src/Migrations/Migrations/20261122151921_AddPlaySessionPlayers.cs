using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaySessionPlayers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "start_player_id",
                table: "play_sessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "play_session_players",
                columns: table => new
                {
                    play_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    players_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_play_session_players", x => new { x.play_session_id, x.players_id });
                    table.ForeignKey(
                        name: "fk_play_session_players_play_sessions_play_session_id",
                        column: x => x.play_session_id,
                        principalTable: "play_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_play_session_players_players_players_id",
                        column: x => x.players_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_play_sessions_start_player_id",
                table: "play_sessions",
                column: "start_player_id");

            migrationBuilder.CreateIndex(
                name: "ix_play_session_players_players_id",
                table: "play_session_players",
                column: "players_id");

            migrationBuilder.AddForeignKey(
                name: "fk_play_sessions_players_start_player_id",
                table: "play_sessions",
                column: "start_player_id",
                principalTable: "players",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_play_sessions_players_start_player_id",
                table: "play_sessions");

            migrationBuilder.DropTable(
                name: "play_session_players");

            migrationBuilder.DropIndex(
                name: "ix_play_sessions_start_player_id",
                table: "play_sessions");

            migrationBuilder.DropColumn(
                name: "start_player_id",
                table: "play_sessions");
        }
    }
}
