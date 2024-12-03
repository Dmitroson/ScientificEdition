using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using ScientificEdition.Areas.Admin.Models.Article;
using ScientificEdition.Business;
using ScientificEdition.Business.Constants;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;
using ScientificEdition.Utilities.Files;

namespace ScientificEdition.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticlesController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly ArticleManager articleManager;
        private readonly FileManager fileManager;
        private readonly ApplicationDbContext dbContext;

        public ArticlesController(
            UserManager<User> userManager,
            ArticleManager articleManager,
            FileManager fileManager,
            ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.articleManager = articleManager;
            this.fileManager = fileManager;
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index(string? query, string? status = null, string? category = null,
            string? sortField = null, string? sortOrder = null)
        {
            var articlesQuery = dbContext.Articles
                .Include(a => a.Author)
                .Include(a => a.Category)
                .Include(a => a.JournalEdition)
                .AsQueryable();

            query = query?.Trim();
            if (!string.IsNullOrEmpty(query))
            {
                var querySegments = query.ToLower().Split(' ');
                articlesQuery = articlesQuery
                    .Where(a => querySegments.Any(q => a.Title.ToLower().Contains(q))
                        || querySegments.Any(q => a.Author!.FirstName.ToLower().Contains(q))
                        || querySegments.Any(q => a.Author!.LastName.ToLower().Contains(q)));
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<ArticleStatus>(status, out var articleStatus))
                    articlesQuery = articlesQuery.Where(a => a.Status == articleStatus);
            }

            if (!string.IsNullOrEmpty(category))
                articlesQuery = articlesQuery.Where(a => a.Category!.Name == category);

            articlesQuery = sortField switch
            {
                Sorting.Fields.Title => sortOrder == Sorting.Order.SortDescending
                    ? articlesQuery.OrderByDescending(a => a.Title)
                    : articlesQuery.OrderBy(a => a.Title),

                Sorting.Fields.UploadDate => sortOrder == Sorting.Order.SortDescending
                    ? articlesQuery.OrderByDescending(a => a.UploadDate)
                    : articlesQuery.OrderBy(a => a.UploadDate),

                _ => articlesQuery.OrderByDescending(a => a.UploadDate)
            };

            var articles = articlesQuery.ToList();
            return View(articles);
        }

        [HttpGet]
        public IActionResult Details(Guid id)
        {
            var article = dbContext.Articles
                .Include(a => a.Author)
                .Include(a => a.Category)
                .Include(a => a.Versions)
                .Include(a => a.Reviewers)
                .FirstOrDefault(m => m.Id == id);

            if (article == null)
                return NotFound();

            article.Versions = article.Versions
                .OrderByDescending(v => v.UploadDate)
                .ToList();

            return View(article);
        }

        [HttpGet]
        public IActionResult VersionDetails(Guid id)
        {
            var version = dbContext.ArticleVersions
                .Include(v => v.Article)
                .Include(v => v.Reviews)
                .ThenInclude(r => r.Reviewer)
                .FirstOrDefault(v => v.Id == id);

            if (version == null)
                return NotFound();

            return View(version);
        }

        [HttpGet]
        public IActionResult DownloadVersion(Guid versionId)
        {
            var version = dbContext.ArticleVersions
                .Include(v => v.Article)
                .ThenInclude(a => a!.Author)
                .FirstOrDefault(v => v.Id == versionId);

            if (version == null)
                return NotFound();

            var filePath = version.FilePath;
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var author = version.Article!.Author;
            var authorDownloadingName = $"{author!.LastName}-{author.FirstName}";

            var articleVersionTitle = $"{authorDownloadingName}_{version.Article!.Title}_v{version.VersionNumber}";
            var downloadFileName = $"{articleVersionTitle}{Path.GetExtension(filePath)}";

            return PhysicalFile(filePath, "application/octet-stream", downloadFileName);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var article = await dbContext.Articles.FindAsync(id);
            if (article == null)
                return NotFound();

            var model = new ArticleInputModel
            {
                Id = article.Id,
                Title = article.Title,
                CategoryId = article.CategoryId,
                Status = article.Status
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleInputModel model)
        {
            if (ModelState.IsValid)
            {
                var article = await dbContext.Articles.FindAsync(model.Id);
                if (article == null)
                    return NotFound();

                var category = await dbContext.Categories.FindAsync(model.CategoryId);
                if (category == null)
                    return NotFound();

                article.Title = model.Title;
                article.Category = category;
                article.Status = model.Status;

                dbContext.Update(article);
                await dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult AssignReviewers(Guid articleId)
        {
            var article = articleManager.GetArticle(articleId);
            if (article == null)
                return NotFound();
            if (article.Status != ArticleStatus.New && article.Status != ArticleStatus.Review)
                return NotFound();

            var assignedReviewers = article.Reviewers.Select(r => r.Id).ToList();
            var model = new ReviewersAssignmentModel
            {
                ArticleId = articleId,
                Article = article,
                ReviewerIds = assignedReviewers
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignReviewers(ReviewersAssignmentModel model)
        {
            var article = articleManager.GetArticle(model.ArticleId);
            if (article == null)
                return NotFound();
            if (article.Status != ArticleStatus.New && article.Status != ArticleStatus.Review)
                return NotFound();

            if (!ModelState.IsValid)
            {
                model.Article = article;
                return View(model);
            }

            if (model.ReviewerIds.Count > 2)
            {
                ModelState.AddModelError(nameof(ReviewersAssignmentModel.ReviewerIds), "Потрібно обрати 2 рецензенти.");
                return View(model);
            }

            var selectedUsers = dbContext.Users.Where(u => model.ReviewerIds.Contains(u.Id)).ToList();
            var validReviewers = new List<User>();
            foreach (var user in selectedUsers)
            {
                if (user != null && await userManager.IsInRoleAsync(user, UserRoles.Reviewer))
                    validReviewers.Add(user);
            }

            if (validReviewers.Count < 2)
            {
                ModelState.AddModelError(nameof(ReviewersAssignmentModel.ReviewerIds), "Деякі рецензенти недоступні для вибору.");

                model.ReviewerIds = validReviewers.Select(r => r.Id).ToList();
                return View(model);
            }

            article.Reviewers.Clear();
            article.Reviewers.AddRange(validReviewers);

            if (model.MoveToReview)
                article.Status = ArticleStatus.Review;

            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = article.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var article = await dbContext.Articles
                .Include(a => a.Category)
                .Include(a => a.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null)
                return NotFound();

            return View(article);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var article = await dbContext.Articles
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article != null)
            {
                dbContext.Articles.Remove(article);
                await dbContext.SaveChangesAsync();

                var articleDirectoryPath = GetArticleDirectoryPath(article.Id, article.AuthorId);
                fileManager.DeleteDirectory(articleDirectoryPath);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AddToJournal(Guid articleId)
        {
            var article = await dbContext.Articles
                .Include(a => a.Category)
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == articleId && a.Status == ArticleStatus.Approved);

            if (article == null)
                return NotFound();

            var model = new AddArticleToJournalInputModel
            {
                ArticleId = article.Id,
                Article = article
            };
            return PartialView("_AddToJournal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToJournal(AddArticleToJournalInputModel model)
        {
            var article = await dbContext.Articles
                .Include(a => a.Category)
                .Include(a => a.Author)
                .FirstOrDefaultAsync(a => a.Id == model.ArticleId && a.Status == ArticleStatus.Approved);

            if (article == null)
                return NotFound();

            var journal = await dbContext.JournalEditions
                .Include(a => a.Articles)
                .FirstOrDefaultAsync(j => j.Id == model.JournalId && !j.Published);

            if (journal == null)
            {
                model.Article = article;
                return PartialView("_AddToJournal", model);
            }

            article.Status = ArticleStatus.Scheduled;
            journal.Articles.Add(article);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private static string[] GetArticleDirectoryPath(Guid articleId, string authorId)
        {
            if (articleId == Guid.Empty)
                throw new ArgumentNullException(nameof(articleId));

            return ["Documents", authorId, articleId.ToString()];
        }
    }
}
