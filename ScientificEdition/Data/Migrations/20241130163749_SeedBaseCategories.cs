using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScientificEdition.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedBaseCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Біологія')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Фізика')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Хімія')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Математика')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Інженерія')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Комп''ютерні науки')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Медичні науки')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Психологія')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Соціологія')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Історія')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Філософія')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Література')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Лінгвістика')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Економіка')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Право')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Політологія')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Мистецтво та дизайн')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Музика')");
            migrationBuilder.Sql("INSERT INTO [Categories] ([Id], [Name]) VALUES (NEWID(), N'Екологічні науки')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [Categories]");
        }
    }
}
