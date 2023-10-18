using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Donace_BE_Project.Migrations
{
    /// <inheritdoc />
    public partial class updatecalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSubcribed",
                table: "CalendarParticipations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSubcribed",
                table: "CalendarParticipations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
