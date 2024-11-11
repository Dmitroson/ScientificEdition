using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScientificEdition.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionNumberColumnToArticleVersionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VersionNumber",
                table: "ArticleVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VersionNumber",
                table: "ArticleVersions");
        }
    }
}
