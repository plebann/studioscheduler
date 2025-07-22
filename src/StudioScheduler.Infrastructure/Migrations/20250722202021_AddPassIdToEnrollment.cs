using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPassIdToEnrollment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PassId",
                table: "Enrollments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_PassId",
                table: "Enrollments",
                column: "PassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Passes_PassId",
                table: "Enrollments",
                column: "PassId",
                principalTable: "Passes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Passes_PassId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_PassId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "PassId",
                table: "Enrollments");
        }
    }
}
