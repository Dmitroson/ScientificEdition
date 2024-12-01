using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScientificEdition.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddArticlesReviewersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleReviewers",
                columns: table => new
                {
                    AssignedArticlesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleReviewers", x => new { x.AssignedArticlesId, x.ReviewersId });
                    table.ForeignKey(
                        name: "FK_ArticleReviewers_Articles_AssignedArticlesId",
                        column: x => x.AssignedArticlesId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleReviewers_AspNetUsers_ReviewersId",
                        column: x => x.ReviewersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleReviewers_ReviewersId",
                table: "ArticleReviewers",
                column: "ReviewersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleReviewers");
        }
    }
}
