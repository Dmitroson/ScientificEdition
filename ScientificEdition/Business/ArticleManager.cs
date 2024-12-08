using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;
using System.Security.Authentication;

namespace ScientificEdition.Business
{
    public class ArticleManager
    {
        private readonly ApplicationDbContext dbContext;

        public ArticleManager(ApplicationDbContext dbContext)
            => this.dbContext = dbContext;

        public Article? GetArticle(Guid articleId)
        {
            return dbContext.Articles
                .Include(a => a.Author)
                .Include(a => a.Category)
                .Include(a => a.Reviewers)
                .FirstOrDefault(m => m.Id == articleId);
        }

        public ArticleVersion? GetLastArticleVersion(Guid articleId)
        {
            var article = dbContext.Articles
                .Include(a => a.Versions)
                .FirstOrDefault(a => a.Id == articleId);

            if (article == null)
                return null;

            return article.Versions.MaxBy(v => v.VersionNumber);
        }

        public int GenerateNewArticleVersionNumber(Guid articleId)
        {
            if (articleId == Guid.Empty)
                throw new ArgumentNullException(nameof(articleId));

            var versionsCount = dbContext.ArticleVersions.Where(v => v.ArticleId == articleId).Count();
            return versionsCount + 1;
        }

        public async Task<int> CountArticlesToRework(string userId)
        {
            return await dbContext.Articles
                .CountAsync(a => a.AuthorId.Equals(userId) && a.Status == ArticleStatus.Rework);
        }

        public async Task<int> CountArticlesToReview(string userId)
        {
            var reviewer = await dbContext.Users
                .Include(u => u.Reviews)
                .Include(u => u.AssignedArticles)
                .ThenInclude(a => a.Versions)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (reviewer == null)
                return 0;

            return await dbContext.ArticleVersions
                .CountAsync(v => v.Article!.Status == ArticleStatus.Review
                    && v.Article!.Reviewers.Any(r => r.Id == userId)
                    && !v.Reviews.Any(r => r.ReviewerId == userId));
        }
    }
}
