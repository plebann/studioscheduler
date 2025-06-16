using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudioScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePassToSalsaMeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingClasses",
                table: "Passes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemainingClasses",
                table: "Passes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
