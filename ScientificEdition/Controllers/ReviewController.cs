//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ScientificEdition.Data;
//using ScientificEdition.Models;
//using System.Security.Claims;

//namespace ScientificEdition.Controllers
//{
//    [Authorize(Roles = $"{UserRoles.Reviewer}, {UserRoles.Admin}")]
//    public class ReviewController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public ReviewController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var reviews = await _context.Reviews.Include(r => r.Article).Include(r => r.Reviewer).ToListAsync();
//            return View(reviews);
//        }

//        public IActionResult Create()
//        {
//            var articles = _context.Articles.Where(a => a.Status == "Новий").ToList();
//            ViewBag.Articles = articles;
//            return View();
//        }

//        // POST: Review/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(ReviewViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var review = new Review
//                {
//                    ArticleId = model.ArticleId,
//                    Content = model.Content,
//                    ReviewerId = User.FindFirstValue(ClaimTypes.NameIdentifier), // Отримуємо ID рецензента з контексту
//                    ReviewDate = DateTime.Now
//                };

//                _context.Add(review);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(model);
//        }

//        // GET: Review/Edit/5
//        public async Task<IActionResult> Edit(int id)
//        {
//            var review = await _context.Reviews.FindAsync(id);
//            if (review == null)
//            {
//                return NotFound();
//            }

//            var model = new ReviewViewModel
//            {
//                Id = review.Id,
//                ArticleId = review.ArticleId,
//                Content = review.Comment
//            };

//            return View(model);
//        }

//        // POST: Review/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, ReviewViewModel model)
//        {
//            if (id != model.Id)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                var review = await _context.Reviews.FindAsync(id);
//                if (review == null)
//                {
//                    return NotFound();
//                }

//                review.Comment = model.Content;

//                _context.Update(review);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            return View(model);
//        }

//        // GET: Review/Delete/5
//        [Authorize(Roles = UserRoles.Admin)]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var review = await _context.Reviews.Include(r => r.Article).Include(r => r.Reviewer).FirstOrDefaultAsync(m => m.Id == id);
//            if (review == null)
//            {
//                return NotFound();
//            }

//            return View(review);
//        }

//        // POST: Review/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        [Authorize(Roles = UserRoles.Admin)]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var review = await _context.Reviews.FindAsync(id);
//            if (review != null)
//            {
//                _context.Reviews.Remove(review);
//                await _context.SaveChangesAsync();
//            }
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
