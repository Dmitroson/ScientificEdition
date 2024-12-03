using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScientificEdition.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateJournalEditionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "JournalEditionId",
                table: "Articles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "JournalEditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEditions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_JournalEditionId",
                table: "Articles",
                column: "JournalEditionId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEditions_CategoryId",
                table: "JournalEditions",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_JournalEditions_JournalEditionId",
                table: "Articles",
                column: "JournalEditionId",
                principalTable: "JournalEditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_JournalEditions_JournalEditionId",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "JournalEditions");

            migrationBuilder.DropIndex(
                name: "IX_Articles_JournalEditionId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "JournalEditionId",
                table: "Articles");
        }
    }
}
