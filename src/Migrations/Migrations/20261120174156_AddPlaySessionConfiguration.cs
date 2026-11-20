using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddPlaySessionConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_board_games_play_sessions_play_session_id",
                table: "board_games");

            migrationBuilder.DropIndex(
                name: "ix_board_games_play_session_id",
                table: "board_games");

            migrationBuilder.DropColumn(
                name: "play_session_id",
                table: "board_games");

            migrationBuilder.CreateTable(
                name: "play_session_expansions",
                columns: table => new
                {
                    play_session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    used_expansions_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_play_session_expansions", x => new { x.play_session_id, x.used_expansions_id });
                    table.ForeignKey(
                        name: "fk_play_session_expansions_board_games_used_expansions_id",
                        column: x => x.used_expansions_id,
                        principalTable: "board_games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_play_session_expansions_play_sessions_play_session_id",
                        column: x => x.play_session_id,
                        principalTable: "play_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_play_sessions_created_at",
                table: "play_sessions",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_play_sessions_is_deleted",
                table: "play_sessions",
                column: "is_deleted",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_play_sessions_start_time",
                table: "play_sessions",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "ix_play_session_expansions_used_expansions_id",
                table: "play_session_expansions",
                column: "used_expansions_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "play_session_expansions");

            migrationBuilder.DropIndex(
                name: "ix_play_sessions_created_at",
                table: "play_sessions");

            migrationBuilder.DropIndex(
                name: "ix_play_sessions_is_deleted",
                table: "play_sessions");

            migrationBuilder.DropIndex(
                name: "ix_play_sessions_start_time",
                table: "play_sessions");

            migrationBuilder.AddColumn<Guid>(
                name: "play_session_id",
                table: "board_games",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_board_games_play_session_id",
                table: "board_games",
                column: "play_session_id");

            migrationBuilder.AddForeignKey(
                name: "fk_board_games_play_sessions_play_session_id",
                table: "board_games",
                column: "play_session_id",
                principalTable: "play_sessions",
                principalColumn: "id");
        }
    }
}
