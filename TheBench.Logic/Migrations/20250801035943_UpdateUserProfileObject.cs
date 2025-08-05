using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBench.Logic.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserProfileObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Sports",
                table: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserProfiles",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Schedule",
                table: "UserProfiles",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "UserProfiles",
                newName: "Auth0Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_Auth0Id",
                table: "UserProfiles",
                column: "Auth0Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_Id",
                table: "UserProfiles",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_Auth0Id",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_Id",
                table: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "UserProfiles",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "UserProfiles",
                newName: "Schedule");

            migrationBuilder.RenameColumn(
                name: "Auth0Id",
                table: "UserProfiles",
                newName: "PhoneNumber");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int[]>(
                name: "Sports",
                table: "UserProfiles",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles",
                column: "UserId",
                unique: true);
        }
    }
}
