using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBench.Logic.Migrations
{
    /// <inheritdoc />
    public partial class AddAssociation_ToGameTeamObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SubstituteRequests_GameId",
                table: "SubstituteRequests",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteRequests_TeamId",
                table: "SubstituteRequests",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubstituteRequests_Games_GameId",
                table: "SubstituteRequests",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubstituteRequests_Teams_TeamId",
                table: "SubstituteRequests",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubstituteRequests_Games_GameId",
                table: "SubstituteRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_SubstituteRequests_Teams_TeamId",
                table: "SubstituteRequests");

            migrationBuilder.DropIndex(
                name: "IX_SubstituteRequests_GameId",
                table: "SubstituteRequests");

            migrationBuilder.DropIndex(
                name: "IX_SubstituteRequests_TeamId",
                table: "SubstituteRequests");
        }
    }
}
