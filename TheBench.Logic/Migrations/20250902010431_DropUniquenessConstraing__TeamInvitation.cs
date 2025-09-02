using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBench.Logic.Migrations
{
    /// <inheritdoc />
    public partial class DropUniquenessConstraing__TeamInvitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamInvitations_TeamId_InviteeEmail",
                table: "TeamInvitations");

            migrationBuilder.CreateIndex(
                name: "IX_TeamInvitations_TeamId_InviteeEmail_Status",
                table: "TeamInvitations",
                columns: new[] { "TeamId", "InviteeEmail", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeamInvitations_TeamId_InviteeEmail_Status",
                table: "TeamInvitations");

            migrationBuilder.CreateIndex(
                name: "IX_TeamInvitations_TeamId_InviteeEmail",
                table: "TeamInvitations",
                columns: new[] { "TeamId", "InviteeEmail" },
                unique: true);
        }
    }
}
