using Microsoft.AspNetCore.Mvc;
using ScientificEdition.Areas.Admin.Models.Category;
using ScientificEdition.Business;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;

namespace ScientificEdition.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly CategoryManager categoryManager;
        private readonly ApplicationDbContext dbContext;

        public CategoryController(CategoryManager categoryManager, ApplicationDbContext dbContext)
        {
            this.categoryManager = categoryManager;
            this.dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var categories = categoryManager.GetAllCategories();
            return View(categories);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryInputModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingCategory = dbContext.Categories.FirstOrDefault(c => c.Name.Equals(model.Name));
            if (existingCategory != null)
            {
                ModelState.AddModelError(nameof(CategoryInputModel.Name), "Категорія із такою назвою уже створена.");
                return View(model);
            }

            var category = new Category { Name = model.Name };
            dbContext.Categories.Add(category);
            dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var category = dbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            var model = new CategoryInputModel { Id = category.Id, Name = category.Name };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryInputModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var category = dbContext.Categories.FirstOrDefault(c => c.Id == model.Id);
            if (category == null)
                return NotFound();

            category.Name = model.Name;
            dbContext.Categories.Update(category);
            dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            var category = dbContext.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();

            dbContext.Categories.Remove(category);
            dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
