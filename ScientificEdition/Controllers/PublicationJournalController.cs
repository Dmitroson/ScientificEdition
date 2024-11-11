//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ScientificEdition.Data;
//using ScientificEdition.Models;

//namespace ScientificEdition.Controllers
//{
//    [Authorize(Roles = "Адмін")]
//    public class PublicationJournalController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public PublicationJournalController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: PublicationJournal
//        public async Task<IActionResult> Index()
//        {
//            var publications = await _context.PublicationJournals.Include(p => p.Article).ToListAsync();
//            return View(publications);
//        }

//        // GET: PublicationJournal/Publish/5
//        public async Task<IActionResult> Publish(int id)
//        {
//            var article = await _context.Articles.Include(a => a.Author).FirstOrDefaultAsync(a => a.Id == id);
//            if (article == null)
//            {
//                return NotFound();
//            }

//            var model = new PublicationJournalViewModel
//            {
//                ArticleId = article.Id,
//                Title = article.Title,
//                AuthorName = article.Author.Name, // Припустимо, що у моделі Article є навігаційна властивість до автора
//                ReviewStatus = article.Status
//            };

//            return View(model);
//        }

//        // POST: PublicationJournal/Publish
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Publish(PublicationJournalViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var publication = new PublicationJournal
//                {
//                    ArticleId = model.ArticleId,
//                    PublicationDate = DateTime.Now
//                };

//                _context.PublicationJournals.Add(publication);
//                await _context.SaveChangesAsync();

//                // Оновлення статусу статті на "Опублікована"
//                var article = await _context.Articles.FindAsync(model.ArticleId);
//                if (article != null)
//                {
//                    article.Status = "Опублікована";
//                    _context.Update(article);
//                    await _context.SaveChangesAsync();
//                }

//                return RedirectToAction(nameof(Index));
//            }
//            return View(model);
//        }

//        // GET: PublicationJournal/Delete/5
//        public async Task<IActionResult> Delete(int id)
//        {
//            var publication = await _context.PublicationJournals.Include(p => p.Article).FirstOrDefaultAsync(p => p.Id == id);
//            if (publication == null)
//            {
//                return NotFound();
//            }

//            return View(publication);
//        }

//        // POST: PublicationJournal/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var publication = await _context.PublicationJournals.FindAsync(id);
//            if (publication != null)
//            {
//                _context.PublicationJournals.Remove(publication);
//                await _context.SaveChangesAsync();
//            }
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
