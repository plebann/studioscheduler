using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelWithDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanceClasses_Rooms_RoomId",
                table: "DanceClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_DanceClasses_Users_InstructorId",
                table: "DanceClasses");

            migrationBuilder.DropIndex(
                name: "IX_DanceClasses_InstructorId",
                table: "DanceClasses");

            migrationBuilder.DropIndex(
                name: "IX_DanceClasses_RoomId",
                table: "DanceClasses");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "DanceClasses");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "DanceClasses");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "DanceClasses");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "DanceClasses");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Schedules",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "InstructorId",
                table: "Schedules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "Schedules",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "Schedules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_InstructorId",
                table: "Schedules",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_RoomId",
                table: "Schedules",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Rooms_RoomId",
                table: "Schedules",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Users_InstructorId",
                table: "Schedules",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Rooms_RoomId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Users_InstructorId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_InstructorId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_RoomId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Schedules");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "DanceClasses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "InstructorId",
                table: "DanceClasses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "DanceClasses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "DanceClasses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DanceClasses_InstructorId",
                table: "DanceClasses",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_DanceClasses_RoomId",
                table: "DanceClasses",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_DanceClasses_Rooms_RoomId",
                table: "DanceClasses",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DanceClasses_Users_InstructorId",
                table: "DanceClasses",
                column: "InstructorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
