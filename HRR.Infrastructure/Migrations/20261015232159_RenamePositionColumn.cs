using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRR.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamePositionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Positiopn",
                table: "Employees",
                newName: "Position");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Employees",
                newName: "Positiopn");
        }
    }
}
