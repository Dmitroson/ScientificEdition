using ScientificEdition.Data;

namespace ScientificEdition.Business
{
    public class ArticleManager
    {
        private readonly ApplicationDbContext dbContext;

        public ArticleManager(ApplicationDbContext dbContext)
            => this.dbContext = dbContext;

        public int GenerateNewArticleVersionNumber(Guid articleId)
        {
            if (articleId == Guid.Empty)
                throw new ArgumentNullException(nameof(articleId));

            var versionsCount = dbContext.ArticleVersions.Where(v => v.ArticleId == articleId).Count();
            return versionsCount + 1;
        }
    }
}
