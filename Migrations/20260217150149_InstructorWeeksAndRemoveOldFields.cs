using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkiSettlement.Migrations
{
    /// <inheritdoc />
    public partial class InstructorWeeksAndRemoveOldFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeeksWorked",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "WorkedHours",
                table: "Instructors");

            migrationBuilder.CreateTable(
                name: "InstructorWeeks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InstructorId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeekNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    HoursWorked = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorWeeks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorWeeks_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstructorWeeks_InstructorId",
                table: "InstructorWeeks",
                column: "InstructorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructorWeeks");

            migrationBuilder.AddColumn<int>(
                name: "WeeksWorked",
                table: "Instructors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "WorkedHours",
                table: "Instructors",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
