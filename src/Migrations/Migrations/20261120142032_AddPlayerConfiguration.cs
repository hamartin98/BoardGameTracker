using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "normalized_name",
                table: "players",
                type: "text",
                nullable: false,
                defaultValue: "");

            // Backfill existing rows before the unique index, otherwise they all share ""
            migrationBuilder.Sql("UPDATE players SET normalized_name = lower(name);");

            migrationBuilder.CreateIndex(
                name: "ix_players_created_at",
                table: "players",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_players_is_deleted",
                table: "players",
                column: "is_deleted",
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_players_normalized_name",
                table: "players",
                column: "normalized_name",
                unique: true,
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_players_created_at",
                table: "players");

            migrationBuilder.DropIndex(
                name: "ix_players_is_deleted",
                table: "players");

            migrationBuilder.DropIndex(
                name: "ix_players_normalized_name",
                table: "players");

            migrationBuilder.DropColumn(
                name: "normalized_name",
                table: "players");
        }
    }
}
