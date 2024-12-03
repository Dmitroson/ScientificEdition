using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScientificEdition.Areas.Admin.Models.Article;
using ScientificEdition.Areas.Admin.Models.Journal;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;

namespace ScientificEdition.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class JournalsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public JournalsController(ApplicationDbContext dbContext)
            => this.dbContext = dbContext;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var journals = await dbContext.JournalEditions
                .Include(j => j.Category)
                .Include(j => j.Articles)
                .ToListAsync();

            return View(journals);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var journal = await dbContext.JournalEditions
                .Include(j => j.Category)
                .Include(j => j.Articles)
                .ThenInclude(a => a.Author)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (journal == null)
                return NotFound();

            return View(journal);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(JournalInputModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var journal = new JournalEdition
            {
                Title = model.Title!,
                Description = model.Description,
                CategoryId = model.CategoryId
            };

            dbContext.JournalEditions.Add(journal);
            dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var journal = dbContext.JournalEditions.FirstOrDefault(c => c.Id == id);
            if (journal == null)
                return NotFound();

            var model = new JournalInputModel
            {
                Id = journal.Id,
                CategoryId = journal.CategoryId,
                Title = journal.Title,
                Description = journal.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JournalInputModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var journal = dbContext.JournalEditions.FirstOrDefault(c => c.Id == model.Id);
            if (journal == null)
                return NotFound();

            journal.Title = model.Title!;
            journal.Description = model.Description;

            dbContext.JournalEditions.Update(journal);
            dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            var journal = dbContext.JournalEditions.FirstOrDefault(c => c.Id == id);
            if (journal == null)
                return NotFound();

            dbContext.JournalEditions.Remove(journal);
            dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Publish(Guid id)
        {
            var journal = await dbContext.JournalEditions
                .Include(j => j.Category)
                .FirstOrDefaultAsync(j => j.Id == id && !j.Published);

            if (journal == null)
                return NotFound();

            var model = new JournalPublishInputModel
            {
                JournalId = journal.Id,
                Journal = journal
            };
            return PartialView("_PublishJournal", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(JournalPublishInputModel model)
        {
            var journal = await dbContext.JournalEditions
                .Include(j => j.Category)
                .Include(j => j.Articles)
                .FirstOrDefaultAsync(j => j.Id == model.JournalId && !j.Published);

            if (journal == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            if (model.PublishNow || model.PublishDate.HasValue)
            {
                foreach (var article in journal.Articles)
                    article.Status = ArticleStatus.Published;

                journal.Published = true;
                journal.PublishDate = model.PublishDate ?? DateTime.Now;

                dbContext.JournalEditions.Update(journal);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
