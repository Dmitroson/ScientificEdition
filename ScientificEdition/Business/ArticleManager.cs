using Microsoft.EntityFrameworkCore;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;

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

        public int GenerateNewArticleVersionNumber(Guid articleId)
        {
            if (articleId == Guid.Empty)
                throw new ArgumentNullException(nameof(articleId));

            var versionsCount = dbContext.ArticleVersions.Where(v => v.ArticleId == articleId).Count();
            return versionsCount + 1;
        }
    }
}
