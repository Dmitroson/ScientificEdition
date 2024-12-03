using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScientificEdition.Business;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;
using ScientificEdition.Models.Article;
using ScientificEdition.Utilities.Files;
using System.Security.Authentication;

namespace ScientificEdition.Controllers
{
    [Authorize(Roles = UserRoles.Author)]
    public class ArticleController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly FileManager fileManager;
        private readonly ArticleManager articleManager;
        private readonly ApplicationDbContext dbContext;

        public ArticleController(
            UserManager<User> userManager,
            FileManager fileManager,
            ArticleManager articleManager,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.fileManager = fileManager;
            this.articleManager = articleManager;
            dbContext = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);

            var articles = await dbContext.Articles
                .Where(a => a.AuthorId.Equals(userId))
                .Include(a => a.Author)
                .Include(a => a.Category)
                .Include(a => a.Versions)
                .ThenInclude(v => v.Reviews)
                .Include(a => a.JournalEdition)
                .ToListAsync();

            return View(articles);
        }

        [HttpGet]
        public IActionResult Details(Guid id)
        {
            var article = dbContext.Articles
                .Include(a => a.Category)
                .Include(a => a.Versions)
                .FirstOrDefault(m => m.Id == id);

            if (article == null)
                return NotFound();

            var userId = userManager.GetUserId(User);
            if (User.IsInRole(UserRoles.Author) && article.AuthorId != userId)
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
                .FirstOrDefault(v => v.Id == id);

            if (version == null)
                return NotFound();

            var userId = userManager.GetUserId(User);
            if (User.IsInRole(UserRoles.Author) && version.Article!.AuthorId != userId)
                return NotFound();

            return View(version);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DownloadVersion(Guid versionId)
        {
            var version = dbContext.ArticleVersions
                .Include(v => v.Article)
                .FirstOrDefault(v => v.Id == versionId);

            if (version == null)
                return NotFound();

            var userId = userManager.GetUserId(User);
            if (User.IsInRole(UserRoles.Author) && version.Article!.AuthorId != userId)
                return NotFound();

            var filePath = version.FilePath;
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var articleVersionTitle = $"{version.Article!.Title}_v{version.VersionNumber}";
            var downloadFileName = $"{articleVersionTitle}{Path.GetExtension(filePath)}";

            return PhysicalFile(filePath, "application/octet-stream", downloadFileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleInputModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.File == null || model.File.Length <= 0)
                return View(model);

            var category = await dbContext.Categories.FindAsync(model.CategoryId);
            if (category == null)
                return NotFound();

            var userId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return NotFound();

            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                CategoryId = category.Id,
                UploadDate = DateTime.Now,
                Status = ArticleStatus.New,
                AuthorId = userId
            };
            dbContext.Articles.Add(article);

            var versionNumber = articleManager.GenerateNewArticleVersionNumber(article.Id);
            var filePath = await fileManager.SaveFile(model.File, $"version_{versionNumber}", GetArticleDirectoryPath(article.Id));

            var version = new ArticleVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = versionNumber,
                ArticleId = article.Id,
                FilePath = filePath,
                Comment = model.Comment,
                UploadDate = article.UploadDate
            };
            dbContext.ArticleVersions.Add(version);

            await dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string[] GetArticleDirectoryPath(Guid articleId)
        {
            if (articleId == Guid.Empty)
                throw new ArgumentNullException(nameof(articleId));

            var userId = userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                throw new AuthenticationException();

            return ["Documents", userId, articleId.ToString()];
        }

        [HttpGet]
        public IActionResult UploadVersion(Guid articleId)
        {
            var article = dbContext.Articles
                .Include(a => a.Versions)
                .FirstOrDefault(a => a.Id == articleId);

            if (article == null)
                return NotFound();

            if (article.Status != ArticleStatus.Rework)
                return BadRequest("Нова версія може бути завантажена тільки для статей у статусі 'На доопрацюванні'.");

            var model = new ArticleVersionInputModel { ArticleId = article.Id };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadVersion(ArticleVersionInputModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var article = dbContext.Articles
                .Include(a => a.Versions)
                .FirstOrDefault(a => a.Id == model.ArticleId);

            if (article == null)
                return NotFound();

            if (article.Status != ArticleStatus.Rework)
                return BadRequest("Нова версія може бути завантажена тільки для статей у статусі 'На доопрацюванні'.");

            var versionNumber = articleManager.GenerateNewArticleVersionNumber(article.Id);
            var filePath = await fileManager.SaveFile(model.File!, $"version_{versionNumber}", GetArticleDirectoryPath(article.Id));

            var version = new ArticleVersion
            {
                Id = Guid.NewGuid(),
                VersionNumber = versionNumber,
                ArticleId = article.Id,
                FilePath = filePath,
                Comment = model.Comment,
                UploadDate = DateTime.Now
            };

            article.Status = ArticleStatus.Review;
            dbContext.ArticleVersions.Add(version);
            dbContext.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = article.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var article = await dbContext.Articles.FindAsync(id);
            if (article == null)
                return NotFound();

            var model = new ArticleEditViewModel
            {
                Id = article.Id,
                Title = article.Title,
                CategoryId = article.CategoryId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleEditViewModel model)
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

                dbContext.Update(article);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
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
            var article = await dbContext.Articles.FindAsync(id);
            if (article != null)
            {
                dbContext.Articles.Remove(article);
                await dbContext.SaveChangesAsync();

                var articleDirectoryPath = GetArticleDirectoryPath(article.Id);
                fileManager.DeleteDirectory(articleDirectoryPath);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
