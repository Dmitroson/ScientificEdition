using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;
using ScientificEdition.Models.Journal;

namespace ScientificEdition.Controllers
{
    public class JournalController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public JournalController(ApplicationDbContext dbContext)
            => this.dbContext = dbContext;

        public async Task<IActionResult> Index(Guid categoryId)
        {
            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
                return NotFound();

            var journals = await dbContext.JournalEditions
                .Where(j => j.CategoryId == categoryId && j.Published && j.PublishDate <= DateTime.Now)
                .Include(j => j.Articles)
                .ToListAsync();

            var model = new JournalIndexViewModel
            {
                Category = category,
                Journals = journals
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edition(Guid id)
        {
            var journal = await dbContext.JournalEditions
                .Include(j => j.Category)
                .Include(j => j.Articles)
                .ThenInclude(a => a.Author)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (journal == null || !IsJournalVisible(journal))
                return NotFound();

            return View(journal);
        }

        public async Task<IActionResult> Article(Guid id)
        {
            return View();
        }

        private static bool IsJournalVisible(JournalEdition journal)
            => journal.Published && journal.PublishDate <= DateTime.Now;
    }
}
