using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkiSettlement.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Expenses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "DictionaryItems",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CategoryId",
                table: "Expenses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryItems_CategoryId",
                table: "DictionaryItems",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_DictionaryItems_ExpenseCategories_CategoryId",
                table: "DictionaryItems",
                column: "CategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseCategories_CategoryId",
                table: "Expenses",
                column: "CategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DictionaryItems_ExpenseCategories_CategoryId",
                table: "DictionaryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseCategories_CategoryId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_CategoryId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_DictionaryItems_CategoryId",
                table: "DictionaryItems");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "DictionaryItems");
        }
    }
}
