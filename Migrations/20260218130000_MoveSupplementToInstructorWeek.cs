using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkiSettlement.Migrations
{
    /// <inheritdoc />
    public partial class MoveSupplementToInstructorWeek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tylko dodajemy kolumny do InstructorWeeks (bez usuwania z Instructors,
            // żeby migracja przechodziła także gdy Instructors nigdy nie miało tych kolumn).
            migrationBuilder.AddColumn<decimal>(
                name: "SupplementAmount",
                table: "InstructorWeeks",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SupplementName",
                table: "InstructorWeeks",
                type: "TEXT",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplementAmount",
                table: "InstructorWeeks");

            migrationBuilder.DropColumn(
                name: "SupplementName",
                table: "InstructorWeeks");

            migrationBuilder.AddColumn<decimal>(
                name: "SupplementAmount",
                table: "Instructors",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SupplementName",
                table: "Instructors",
                type: "TEXT",
                maxLength: 200,
                nullable: true);
        }
    }
}
