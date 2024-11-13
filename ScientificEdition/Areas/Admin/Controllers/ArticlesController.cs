using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScientificEdition.Business;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;
using ScientificEdition.Utilities.Files;

namespace ScientificEdition.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticlesController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly FileManager fileManager;
        private readonly ArticleManager articleManager;
        private readonly ApplicationDbContext dbContext;

        public ArticlesController(
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
        public IActionResult Index()
        {
            var articles = dbContext.Articles
                .Include(a => a.Author)
                .ToList();

            return View(articles);
        }

        [HttpGet]
        public IActionResult Details(Guid id)
        {
            var article = dbContext.Articles
                .Include(a => a.Author)
                .Include(a => a.Versions)
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

            var article = version.Article!;
            var articleVersionTitle = $"{article.Title}_v{version.VersionNumber}_{article.Author!.FullName}";
            var downloadFileName = $"{articleVersionTitle}{Path.GetExtension(filePath)}";

            return PhysicalFile(filePath, "application/octet-stream", downloadFileName);
        }
    }
}
