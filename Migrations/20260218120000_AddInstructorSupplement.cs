using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkiSettlement.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructorSupplement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplementName",
                table: "Instructors",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SupplementAmount",
                table: "Instructors",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplementName",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "SupplementAmount",
                table: "Instructors");
        }
    }
}
