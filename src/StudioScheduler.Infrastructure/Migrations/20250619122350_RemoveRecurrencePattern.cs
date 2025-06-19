using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRecurrencePattern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecurrencePattern",
                table: "Schedules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecurrencePattern",
                table: "Schedules",
                type: "TEXT",
                nullable: true);
        }
    }
}
