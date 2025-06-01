using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApp.Migrations
{
    /// <inheritdoc />
    public partial class aaddedcatalogad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatalogId",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Catalogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CatalogId",
                table: "Questions",
                column: "CatalogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Catalogs_CatalogId",
                table: "Questions",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Catalogs_CatalogId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "Catalogs");

            migrationBuilder.DropIndex(
                name: "IX_Questions_CatalogId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CatalogId",
                table: "Questions");
        }
    }
}
