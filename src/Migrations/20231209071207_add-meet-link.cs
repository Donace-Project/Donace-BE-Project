using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donace_BE_Project.Migrations
{
    /// <inheritdoc />
    public partial class addmeetlink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LinkMeet",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LinkMeet",
                table: "Events");
        }
    }
}
