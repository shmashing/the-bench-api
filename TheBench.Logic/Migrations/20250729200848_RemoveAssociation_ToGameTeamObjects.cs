using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBench.Logic.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAssociation_ToGameTeamObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "SubstituteRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Sport",
                table: "SubstituteRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "SubstituteRequests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TeamName",
                table: "SubstituteRequests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituteRequests_TeamId_GameId",
                table: "SubstituteRequests",
                columns: new[] { "TeamId", "GameId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubstituteRequests_TeamId_GameId",
                table: "SubstituteRequests");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "SubstituteRequests");

            migrationBuilder.DropColumn(
                name: "Sport",
                table: "SubstituteRequests");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "SubstituteRequests");

            migrationBuilder.DropColumn(
                name: "TeamName",
                table: "SubstituteRequests");

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
    }
}
