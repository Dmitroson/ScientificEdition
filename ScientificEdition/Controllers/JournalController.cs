using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScientificEdition.Business;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;
using ScientificEdition.Models.Journal;
using ScientificEdition.Utilities.Files;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using System.Security.Authentication;

namespace ScientificEdition.Controllers
{
    public class JournalController : Controller
    {
        private readonly ArticleManager articleManager;
        private readonly ApplicationDbContext dbContext;
        private readonly FileManager fileManager;

        public JournalController(
            ArticleManager articleManager,
            ApplicationDbContext dbContext,
            FileManager fileManager)
        {
            this.articleManager = articleManager;
            this.dbContext = dbContext;
            this.fileManager = fileManager;
        }

        public async Task<IActionResult> Index(Guid categoryId)
        {
            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
                return NotFound();

            var journals = await dbContext.JournalEditions
                .Where(j => j.CategoryId == categoryId && j.Published && j.PublishDate <= DateTime.Now)
                .Include(j => j.Articles)
                .ToListAsync();

            var model = new JournalIndexViewModel
            {
                Category = category,
                Journals = journals
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edition(Guid id)
        {
            var journal = await dbContext.JournalEditions
                .Include(j => j.Category)
                .Include(j => j.Articles)
                .ThenInclude(a => a.Author)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (journal == null || !IsJournalVisible(journal))
                return NotFound();

            return View(journal);
        }

        [HttpGet]
        public async Task<IActionResult> Article(Guid id)
        {
            var article = await dbContext.Articles
                .Include(a => a.Author)
                .Include(a => a.JournalEdition)
                .FirstOrDefaultAsync(a => a.Id == id && a.Status == ArticleStatus.Published);

            if (article == null)
                return NotFound();

            var publishedVersion = articleManager.GetLastArticleVersion(id);
            if (publishedVersion == null)
                return NotFound();

            var downloadFileName = $"{article.JournalEdition?.Title}_{article.Title}_{article.Author?.FullName}.pdf";
            var fileCheckResult = ArticlePdfFileAlreadyExists(article);
            if (fileCheckResult.Exists)
                return PhysicalFile(fileCheckResult.Path, "application/pdf", downloadFileName);

            using (var docxStream = new FileStream(Path.GetFullPath(publishedVersion.FilePath), FileMode.Open, FileAccess.Read))
            using (var wordDocument = new WordDocument(docxStream, FormatType.Docx))
            using (var renderer = new DocIORenderer())
            {
                var pdfDocument = renderer.ConvertToPDF(wordDocument);

                var fileName = $"{article.Id}";
                var articleDirectoryPath = GetArticleDirectoryPath(article.AuthorId);

                var fullFilePath = fileManager.SavePdfFile(pdfDocument, fileName, articleDirectoryPath);
                if (string.IsNullOrEmpty(fullFilePath))
                    return BadRequest();

                return PhysicalFile(fullFilePath, "application/pdf", downloadFileName);
            }
        }

        private static bool IsJournalVisible(JournalEdition journal)
            => journal.Published && journal.PublishDate <= DateTime.Now;

        private (bool Exists, string Path) ArticlePdfFileAlreadyExists(Article article)
        {
            var fileNameWithExtension = $"{article.Id}.pdf";
            var articleDirectoryPath = GetArticleDirectoryPath(article.AuthorId);

            var fullFilePath = Path.Combine(fileManager.GetFullPath(articleDirectoryPath), fileNameWithExtension);
            return (System.IO.File.Exists(fullFilePath), fullFilePath);
        }

        public string[] GetArticleDirectoryPath(string authorId)
        {
            if (string.IsNullOrEmpty(authorId))
                throw new AuthenticationException();

            return ["Documents", authorId];
        }
    }
}
