using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBench.Logic.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBoolToEnumForStatus__TeamInvitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "TeamInvitations");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TeamInvitations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TeamInvitations");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "TeamInvitations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
