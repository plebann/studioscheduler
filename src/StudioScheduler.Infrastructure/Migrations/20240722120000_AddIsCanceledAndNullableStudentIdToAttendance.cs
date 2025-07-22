using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudioScheduler.Infrastructure.Migrations
{
    public partial class AddIsCanceledAndNullableStudentIdToAttendance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCanceled",
                table: "Attendances",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "Attendances",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCanceled",
                table: "Attendances");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "Attendances",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
