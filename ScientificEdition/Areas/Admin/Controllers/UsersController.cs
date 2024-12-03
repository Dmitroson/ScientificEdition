using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScientificEdition.Areas.Admin.Models.Users;
using ScientificEdition.Data;
using ScientificEdition.Data.Entities;

namespace ScientificEdition.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext dbContext;

        public UsersController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return NotFound();

            var roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists)
                return NotFound($"Role '{role}' does not exist.");

            var users = await userManager.GetUsersInRoleAsync(role);

            if (role == UserRoles.Reviewer)
            {
                var allCategories = await dbContext.Categories.Include(c => c.Users).ToListAsync();
                foreach (var user in users)
                    user.Categories = allCategories.Where(c => c.Users.Any(u => u.Id == user.Id)).ToList();
            }

            var model = new UsersListViewModel
            {
                Role = role,
                Users = users
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await dbContext.Users
                .Include(u => u.Categories)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            var role = await GetUserRoleName(user);
            var model = new UserUpdateInputModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Role = role,
                CategoryIds = role == UserRoles.Reviewer ? user.Categories.Select(c => c.Id).ToList() : []
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserUpdateInputModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            if (model.Role == UserRoles.Reviewer && model.CategoryIds.Count == 0)
                return View(model);

            var user = await dbContext.Users
                .Include(u => u.Categories)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.Email;
            user.Email = model.Email;

            var result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var role = await GetUserRoleName(user);
                if (model.Role != role)
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                    await userManager.RemoveFromRolesAsync(user, userRoles);
                    await userManager.AddToRoleAsync(user, model.Role!);
                }

                if (model.Role == UserRoles.Reviewer)
                {
                    user.Categories.Clear();

                    var categories = await dbContext.Categories.Where(c => model.CategoryIds.Contains(c.Id)).ToListAsync();
                    user.Categories = categories;

                    await dbContext.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index), new { role = model.Role });
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var role = await GetUserRoleName(user);
            return View((user, role));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var role = await GetUserRoleName(user);

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
                return RedirectToAction(nameof(Index), new { role });

            return View((user, role));
        }

        private async Task<string?> GetUserRoleName(User user)
        {
            if (user == null)
                return string.Empty;

            var roles = await userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }
    }
}
