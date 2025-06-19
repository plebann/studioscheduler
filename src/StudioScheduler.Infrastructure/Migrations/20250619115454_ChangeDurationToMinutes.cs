using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDurationToMinutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add a temporary column to store the converted duration in minutes
            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Schedules",
                type: "INTEGER",
                nullable: false,
                defaultValue: 60);

            // Convert existing TimeSpan durations to minutes
            // This SQL extracts the total minutes from TimeSpan format (e.g., "01:00:00" -> 60)
            migrationBuilder.Sql(@"
                UPDATE Schedules 
                SET DurationMinutes = 
                    CASE 
                        WHEN Duration LIKE '__:__:__' THEN 
                            CAST(SUBSTR(Duration, 1, 2) AS INTEGER) * 60 + 
                            CAST(SUBSTR(Duration, 4, 2) AS INTEGER)
                        WHEN Duration LIKE '_:__:__' THEN 
                            CAST(SUBSTR(Duration, 1, 1) AS INTEGER) * 60 + 
                            CAST(SUBSTR(Duration, 3, 2) AS INTEGER)
                        ELSE 60
                    END
                WHERE Duration IS NOT NULL");

            // Drop the old Duration column
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Schedules");

            // Rename the temporary column to Duration
            migrationBuilder.RenameColumn(
                name: "DurationMinutes",
                table: "Schedules",
                newName: "Duration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add a temporary column to store the converted duration as TimeSpan
            migrationBuilder.AddColumn<string>(
                name: "DurationTimeSpan",
                table: "Schedules",
                type: "TEXT",
                nullable: false,
                defaultValue: "01:00:00");

            // Convert minutes back to TimeSpan format
            migrationBuilder.Sql(@"
                UPDATE Schedules 
                SET DurationTimeSpan = 
                    PRINTF('%02d:%02d:00', Duration / 60, Duration % 60)
                WHERE Duration IS NOT NULL");

            // Drop the integer Duration column
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Schedules");

            // Rename the temporary column back to Duration
            migrationBuilder.RenameColumn(
                name: "DurationTimeSpan",
                table: "Schedules",
                newName: "Duration");

            // Change the column type back to TimeSpan
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "Schedules",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
