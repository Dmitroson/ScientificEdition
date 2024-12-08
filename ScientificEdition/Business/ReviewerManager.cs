using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;

namespace ScientificEdition.Business
{
    public class ReviewerManager
    {
        private readonly UserManager<User> userManager;
        private readonly ApplicationDbContext dbContext;

        public ReviewerManager(UserManager<User> userManager, ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.dbContext = dbContext;
        }

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
                .Include(u => u.AssignedArticles)
                .OrderBy(u => u.AssignedArticles.Count(a => a.Status == ArticleStatus.Review))
                .Take(takeCount)
                .ToListAsync();

            return availableReviewers;
        }
    }
}
