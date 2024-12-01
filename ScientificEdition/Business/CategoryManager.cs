using ScientificEdition.Data;
using ScientificEdition.Data.Entities;

namespace ScientificEdition.Business
{
    public class CategoryManager
    {
        private readonly ApplicationDbContext dbContext;

        public CategoryManager(ApplicationDbContext dbContext)
            => this.dbContext = dbContext;

        public List<Category> GetAllCategories()
        {
            var categories = dbContext.Categories.OrderBy(c => c.Name);
            return [.. categories];
        }
    }
}
