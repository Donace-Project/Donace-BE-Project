using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donace_BE_Project.Migrations
{
    /// <inheritdoc />
    public partial class adddescriptioncalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Calendars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Calendars");
        }
    }
}
