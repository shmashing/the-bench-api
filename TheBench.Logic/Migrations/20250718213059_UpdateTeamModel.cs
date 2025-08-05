using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBench.Logic.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeamModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Teams",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Teams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sport",
                table: "Teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Sport",
                table: "Teams");
        }
    }
}
