using Microsoft.EntityFrameworkCore.Migrations;

namespace StudioScheduler.Infrastructure.Migrations;

public partial class MoveLevelAndInstructorToSchedule : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Step 1: Add new columns to Schedule table
        migrationBuilder.AddColumn<string>(
            name: "Level",
            table: "Schedules",
            type: "TEXT",
            nullable: false,
            defaultValue: "P1"); // Default level for existing schedules

        migrationBuilder.AddColumn<Guid>(
            name: "InstructorId",
            table: "Schedules",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "RoomId",
            table: "Schedules",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Capacity",
            table: "Schedules",
            type: "INTEGER",
            nullable: false,
            defaultValue: 20); // Default capacity for existing schedules

        // Step 2: Create indexes for new foreign keys
        migrationBuilder.CreateIndex(
            name: "IX_Schedules_InstructorId",
            table: "Schedules",
            column: "InstructorId");

        migrationBuilder.CreateIndex(
            name: "IX_Schedules_RoomId",
            table: "Schedules",
            column: "RoomId");

        // Step 3: Add foreign key constraints
        migrationBuilder.AddForeignKey(
            name: "FK_Schedules_Users_InstructorId",
            table: "Schedules",
            column: "InstructorId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Schedules_Rooms_RoomId",
            table: "Schedules",
            column: "RoomId",
            principalTable: "Rooms",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        // Step 4: Migrate data from DanceClass to Schedule
        // This SQL will copy the level, instructor, room, and capacity from DanceClass to Schedule
        migrationBuilder.Sql(@"
            UPDATE Schedules 
            SET 
                Level = (
                    SELECT Level 
                    FROM DanceClasses 
                    WHERE DanceClasses.Id = Schedules.DanceClassId
                ),
                InstructorId = (
                    SELECT InstructorId 
                    FROM DanceClasses 
                    WHERE DanceClasses.Id = Schedules.DanceClassId
                ),
                RoomId = (
                    SELECT RoomId 
                    FROM DanceClasses 
                    WHERE DanceClasses.Id = Schedules.DanceClassId
                ),
                Capacity = (
                    SELECT Capacity 
                    FROM DanceClasses 
                    WHERE DanceClasses.Id = Schedules.DanceClassId
                )
            WHERE EXISTS (
                SELECT 1 
                FROM DanceClasses 
                WHERE DanceClasses.Id = Schedules.DanceClassId
            );
        ");

        // Step 5: Remove old columns from DanceClass table
        migrationBuilder.DropColumn(
            name: "Level",
            table: "DanceClasses");

        migrationBuilder.DropColumn(
            name: "InstructorId",
            table: "DanceClasses");

        migrationBuilder.DropColumn(
            name: "RoomId",
            table: "DanceClasses");

        migrationBuilder.DropColumn(
            name: "Capacity",
            table: "DanceClasses");

        // Step 6: Remove old foreign key constraints and indexes
        migrationBuilder.DropIndex(
            name: "IX_DanceClasses_InstructorId",
            table: "DanceClasses");

        migrationBuilder.DropIndex(
            name: "IX_DanceClasses_RoomId",
            table: "DanceClasses");

        migrationBuilder.DropForeignKey(
            name: "FK_DanceClasses_Users_InstructorId",
            table: "DanceClasses");

        migrationBuilder.DropForeignKey(
            name: "FK_DanceClasses_Rooms_RoomId",
            table: "DanceClasses");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Step 1: Add back old columns to DanceClass table
        migrationBuilder.AddColumn<string>(
            name: "Level",
            table: "DanceClasses",
            type: "TEXT",
            nullable: false,
            defaultValue: "P1");

        migrationBuilder.AddColumn<Guid>(
            name: "InstructorId",
            table: "DanceClasses",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "RoomId",
            table: "DanceClasses",
            type: "TEXT",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Capacity",
            table: "DanceClasses",
            type: "INTEGER",
            nullable: false,
            defaultValue: 20);

        // Step 2: Create old indexes
        migrationBuilder.CreateIndex(
            name: "IX_DanceClasses_InstructorId",
            table: "DanceClasses",
            column: "InstructorId");

        migrationBuilder.CreateIndex(
            name: "IX_DanceClasses_RoomId",
            table: "DanceClasses",
            column: "RoomId");

        // Step 3: Add old foreign key constraints
        migrationBuilder.AddForeignKey(
            name: "FK_DanceClasses_Users_InstructorId",
            table: "DanceClasses",
            column: "InstructorId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_DanceClasses_Rooms_RoomId",
            table: "DanceClasses",
            column: "RoomId",
            principalTable: "Rooms",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        // Step 4: Migrate data back from Schedule to DanceClass
        // Note: This is a simplified rollback - it takes the first schedule's data for each dance class
        migrationBuilder.Sql(@"
            UPDATE DanceClasses 
            SET 
                Level = (
                    SELECT Level 
                    FROM Schedules 
                    WHERE Schedules.DanceClassId = DanceClasses.Id 
                    LIMIT 1
                ),
                InstructorId = (
                    SELECT InstructorId 
                    FROM Schedules 
                    WHERE Schedules.DanceClassId = DanceClasses.Id 
                    LIMIT 1
                ),
                RoomId = (
                    SELECT RoomId 
                    FROM Schedules 
                    WHERE Schedules.DanceClassId = DanceClasses.Id 
                    LIMIT 1
                ),
                Capacity = (
                    SELECT Capacity 
                    FROM Schedules 
                    WHERE Schedules.DanceClassId = DanceClasses.Id 
                    LIMIT 1
                )
            WHERE EXISTS (
                SELECT 1 
                FROM Schedules 
                WHERE Schedules.DanceClassId = DanceClasses.Id
            );
        ");

        // Step 5: Remove new columns from Schedule table
        migrationBuilder.DropColumn(
            name: "Level",
            table: "Schedules");

        migrationBuilder.DropColumn(
            name: "InstructorId",
            table: "Schedules");

        migrationBuilder.DropColumn(
            name: "RoomId",
            table: "Schedules");

        migrationBuilder.DropColumn(
            name: "Capacity",
            table: "Schedules");

        // Step 6: Remove new foreign key constraints and indexes
        migrationBuilder.DropIndex(
            name: "IX_Schedules_InstructorId",
            table: "Schedules");

        migrationBuilder.DropIndex(
            name: "IX_Schedules_RoomId",
            table: "Schedules");

        migrationBuilder.DropForeignKey(
            name: "FK_Schedules_Users_InstructorId",
            table: "Schedules");

        migrationBuilder.DropForeignKey(
            name: "FK_Schedules_Rooms_RoomId",
            table: "Schedules");
    }
} 