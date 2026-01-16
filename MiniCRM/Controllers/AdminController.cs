using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MiniCRM.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Admin/WhoAmI
        [HttpGet]
        public async Task<IActionResult> WhoAmI()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Content("Nie jesteś zalogowany.");

            var roles = await _userManager.GetRolesAsync(user);
            return Content($"Zalogowany jako: {user.Email ?? user.UserName}\nRole: {string.Join(", ", roles)}");
        }
    }
}