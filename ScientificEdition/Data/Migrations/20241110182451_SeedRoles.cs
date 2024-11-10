using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScientificEdition.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName]) VALUES (NEWID(), 'Author', 'AUTHOR')");
            migrationBuilder.Sql("INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName]) VALUES (NEWID(), 'Admin', 'ADMIN')");
            migrationBuilder.Sql("INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName]) VALUES (NEWID(), 'Reviewer', 'REVIEWER')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [AspNetRoles] WHERE [Name] IN ('Author', 'Admin', 'Reviewer')");
        }
    }
}
