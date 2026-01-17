using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniCRM.Models;

namespace MiniCRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
            _signInManager = signInManager;
        }

        // GET: /Admin/Users
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            if (!_env.IsDevelopment()) return Forbid();

            var users = _userManager.Users.ToList();
            var vm = new List<AdminUserViewModel>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                vm.Add(new AdminUserViewModel { Id = u.Id, Email = u.Email, Roles = roles.ToList() });
            }
            return View(vm);
        }

        // POST: /Admin/GrantAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GrantAdmin(string id)
        {
            if (!_env.IsDevelopment()) return Forbid();
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var res = await _userManager.AddToRoleAsync(user, "Admin");
                if (res.Succeeded) TempData["Success"] = "Nadano rolę Admin.";
                else TempData["Error"] = "Nie udało się nadać roli.";
            }
            return RedirectToAction("Users");
        }

        // POST: /Admin/RevokeAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeAdmin(string id)
        {
            if (!_env.IsDevelopment()) return Forbid();
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var res = await _userManager.RemoveFromRoleAsync(user, "Admin");
                if (res.Succeeded) TempData["Success"] = "Usunięto rolę Admin.";
                else TempData["Error"] = "Nie udało się usunąć roli.";
            }
            return RedirectToAction("Users");
        }
    }
}