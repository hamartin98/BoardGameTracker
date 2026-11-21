using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerScoreConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_player_scores_session_id",
                table: "player_scores");

            migrationBuilder.CreateIndex(
                name: "ix_player_scores_created_at",
                table: "player_scores",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_player_scores_is_deleted",
                table: "player_scores",
                column: "is_deleted",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_player_scores_session_id_player_id",
                table: "player_scores",
                columns: new[] { "session_id", "player_id" },
                unique: true,
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_player_scores_created_at",
                table: "player_scores");

            migrationBuilder.DropIndex(
                name: "ix_player_scores_is_deleted",
                table: "player_scores");

            migrationBuilder.DropIndex(
                name: "ix_player_scores_session_id_player_id",
                table: "player_scores");

            migrationBuilder.CreateIndex(
                name: "ix_player_scores_session_id",
                table: "player_scores",
                column: "session_id");
        }
    }
}
