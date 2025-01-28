using Blog_Zaliczeniowy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog_Zaliczeniowy.Controllers
{
	[Authorize(Roles = "Administrator")]
	public class AdminController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AdminController(UserManager<ApplicationUser> userManager,
							   RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		// Przykładowa metoda: lista użytkowników
		public async Task<IActionResult> Users()
		{
			var users = _userManager.Users.ToList();
			return View(users);
		}

		// Przykładowa metoda: tworzenie roli
		[HttpPost]
		public async Task<IActionResult> CreateRole(string roleName)
		{
			if (!await _roleManager.RoleExistsAsync(roleName))
			{
				await _roleManager.CreateAsync(new IdentityRole(roleName));
			}
			return RedirectToAction("Roles");
		}

		// ...
	}
}
