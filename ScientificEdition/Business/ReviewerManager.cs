using Microsoft.EntityFrameworkCore;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;

namespace ScientificEdition.Business
{
    public class ReviewerManager
    {
        private readonly ApplicationDbContext dbContext;

        public ReviewerManager(ApplicationDbContext dbContext)
            => this.dbContext = dbContext;

        public async Task<List<User>> GetAvailableReviewersForArticle(Guid articleId, int takeCount = 20)
        {
            var article = dbContext.Articles
                .Include(a => a.Category)
                .Include(a => a.Author)
                .FirstOrDefault(m => m.Id == articleId);

            if (article == null)
                return [];

            var availableReviewers = await dbContext.Users
                .Where(user => dbContext.UserRoles
                    .Where(userRole => userRole.UserId == user.Id)
                    .Join(dbContext.Roles,
                        userRole => userRole.RoleId,
                        role => role.Id,
                        (userRole, role) => role)
                    .Any(role => role.Name == UserRoles.Reviewer) &&
                    user.Categories.Any(category => category.Id == article.CategoryId))
                .OrderBy(u => u.AssignedArticles.Count)
                .Take(takeCount)
                .ToListAsync();

            return availableReviewers;
        }
    }
}
