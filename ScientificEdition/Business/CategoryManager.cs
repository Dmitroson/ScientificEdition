using Microsoft.EntityFrameworkCore;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;

namespace ScientificEdition.Business
{
    public class CategoryManager
    {
        private readonly ApplicationDbContext dbContext;

        public CategoryManager(ApplicationDbContext dbContext)
            => this.dbContext = dbContext;

        public List<Category> GetAllCategories(bool includeRelatedEntities = false)
        {
            var categories = dbContext.Categories.AsQueryable();

            if (includeRelatedEntities)
            {
                categories = categories
                    .Include(c => c.Articles)
                    .Include(c => c.Users);
            }

            return [.. categories.OrderBy(c => c.Name)];
        }
    }
}
