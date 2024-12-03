using Microsoft.EntityFrameworkCore;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;

namespace ScientificEdition.Business
{
    public class JournalManager
    {
        private readonly ApplicationDbContext dbContext;

        public JournalManager(ApplicationDbContext dbContext)
            => this.dbContext = dbContext;

        public async Task<List<JournalEdition>> GetJournalsAsync(Guid categoryId, bool ignorePublished = false)
        {
            var journalsQuery = dbContext.JournalEditions
                .Include(j => j.Category)
                .Include(j => j.Articles)
                .Where(j => j.CategoryId == categoryId)
                .AsQueryable();

            if (!ignorePublished)
                journalsQuery = journalsQuery.Where(j => !j.Published);

            return await journalsQuery.ToListAsync();
        }

        public async Task<List<Category>> GetJournalCategoriesAsync()
        {
            return await dbContext.Categories
                .Where(c => c.Journals.Any(j => j.Published && j.PublishDate <= DateTime.Now))
                .ToListAsync();
        }
    }
}
