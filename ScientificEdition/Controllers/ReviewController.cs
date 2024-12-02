using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;
using ScientificEdition.Models.Review;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;

namespace ScientificEdition.Controllers
{
    [Authorize(Roles = UserRoles.Reviewer)]
    public class ReviewController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext dbContext;

        protected string UserId => userManager.GetUserId(User)!;

        public ReviewController(UserManager<User> userManager, ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var assignedArticles = await dbContext.Articles
                .Include(a => a.Category)
                .Include(a => a.Versions)
                .ThenInclude(v => v.Reviews)
                .Where(a => a.Reviewers.Any(r => r.Id == UserId))
                .ToListAsync();

            return View(assignedArticles);
        }

        [HttpGet]
        public async Task<IActionResult> ArticleDetails(Guid id)
        {
            var article = await dbContext.Articles
                .Include(a => a.Category)
                .Include(a => a.Versions)
                .ThenInclude(v => v.Reviews)
                .FirstOrDefaultAsync(a => a.Id == id && a.Reviewers.Any(r => r.Id == UserId));

            if (article == null)
                return NotFound();

            article.Versions = [.. article.Versions.OrderByDescending(v => v.UploadDate)];

            return View(article);
        }

        [HttpGet]
        public IActionResult DownloadArticleVersion(Guid versionId)
        {
            var version = dbContext.ArticleVersions
                .Include(v => v.Article)
                .FirstOrDefault(v => v.Id == versionId && v.Article!.Reviewers.Any(r => r.Id == UserId));

            if (version == null)
                return NotFound();

            var filePath = version.FilePath;
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var articleVersionTitle = $"{version.Article!.Title}_v{version.VersionNumber}_{version.UploadDate:dd.MM.yyyy}";
            var downloadFileName = $"{articleVersionTitle}{Path.GetExtension(filePath)}";

            return PhysicalFile(filePath, "application/octet-stream", downloadFileName);
        }

        [HttpGet]
        public IActionResult CompareVersions(Guid articleId, int originalVersionId, int revisedVersionId)
        {
            if (originalVersionId == default && revisedVersionId == default)
                return NotFound();

            var article = dbContext.Articles
                .Include(a => a.Versions)
                .FirstOrDefault(a => a.Id == articleId && a.Reviewers.Any(r => r.Id == UserId));

            if (article == null)
                return NotFound();

            var originalVersion = article.Versions.FirstOrDefault(v => v.VersionNumber == originalVersionId);
            var revisedVersion = article.Versions.FirstOrDefault(v => v.VersionNumber == revisedVersionId);
            if (originalVersion == null || revisedVersion == null)
                return NotFound();

            if (!System.IO.File.Exists(originalVersion.FilePath) || !System.IO.File.Exists(revisedVersion.FilePath))
                return NotFound();

            using (var originalDocumentStream = new FileStream(Path.GetFullPath(originalVersion.FilePath), FileMode.Open, FileAccess.Read))
            using (var originalDocument = new WordDocument(originalDocumentStream, FormatType.Docx))
            using (var revisedDocumentStream = new FileStream(Path.GetFullPath(revisedVersion.FilePath), FileMode.Open, FileAccess.Read))
            using (var revisedDocument = new WordDocument(revisedDocumentStream, FormatType.Docx))
            {
                originalDocument.Compare(revisedDocument);

                var stream = new MemoryStream();
                originalDocument.Save(stream, FormatType.Docx);

                stream.Position = 0;

                return File(stream, "application/docx", $"{article.Title}_v{originalVersionId}-v{revisedVersionId}.docx");
            }

        }

        [HttpGet]
        public async Task<IActionResult> ReviewVersion(Guid id)
        {
            var articleVersion = await GetArticleVersion(id);
            if (articleVersion == null)
                return NotFound();

            if (articleVersion.Reviews.Any(r => r.ReviewerId == UserId))
                return RedirectToAction(nameof(ArticleDetails), new { id = articleVersion.ArticleId });

            var model = new ArticleVersionReviewModel
            {
                ArticleVersionId = articleVersion.Id,
                ArticleVersion = articleVersion
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewVersion(ArticleVersionReviewModel model)
        {
            var articleVersion = await GetArticleVersion(model.ArticleVersionId);
            if (articleVersion == null)
                return NotFound();

            if (articleVersion.Reviews.Any(r => r.ReviewerId == UserId))
                return RedirectToAction(nameof(ArticleDetails), new { id = articleVersion.ArticleId });

            if (!ModelState.IsValid)
            {
                model.ArticleVersion = articleVersion;
                return View(model);
            }

            var review = new Review
            {
                ArticleVersionId = articleVersion.Id,
                ReviewerId = UserId,
                Comment = model.ReviewComment!,
                Result = model.Result,
                ReviewDate = DateTime.Now
            };
            articleVersion.Reviews.Add(review);

            if (articleVersion.Reviews.Count >= 2)
            {
                var article = await dbContext.Articles.FirstOrDefaultAsync(a => a.Id == articleVersion.ArticleId);
                if (article == null)
                    return NotFound();

                article.Status = DetermineNextArticleStatus(articleVersion);
            }

            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(ArticleDetails), new { id = articleVersion.Article!.Id });
        }

        private async Task<ArticleVersion?> GetArticleVersion(Guid versionId)
        {
            return await dbContext.ArticleVersions
                .Include(v => v.Article)
                .Include(v => v.Article!.Category)
                .Include(v => v.Article!.Reviewers)
                .Include(a => a.Reviews)
                .FirstOrDefaultAsync(v => v.Id == versionId
                    && v.Article!.Reviewers.Any(r => r.Id == UserId));
        }

        private static ArticleStatus DetermineNextArticleStatus(ArticleVersion version)
        {
            var reviews = version.Reviews.ToList();
            if (reviews.Any(r => r.Result == ReviewResult.Rework))
                return ArticleStatus.Rework;

            if (reviews.All(r => r.Result == ReviewResult.Approved))
                return ArticleStatus.Approved;

            throw new NotSupportedException($"Article version '{version.Id}' has reviews with uknown result type.");
        }
    }
}
