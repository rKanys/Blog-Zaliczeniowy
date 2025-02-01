using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Blog_Zaliczeniowy.Data;
using Blog_Zaliczeniowy.Models;
using Microsoft.AspNetCore.Authorization;

namespace Blog_Zaliczeniowy.Controllers
{
	public class ProfileController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;

		public ProfileController(UserManager<ApplicationUser> userManager,
								 ApplicationDbContext context)
		{
			_userManager = userManager;
			_context = context;
		}

		// GET: /Profile/{userId}
		public async Task<IActionResult> Index(string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				if (User.Identity.IsAuthenticated)
				{
					var currentUserId = _userManager.GetUserId(User);
					return RedirectToAction("Index", new { userId = currentUserId });
				}
				else
				{
					return RedirectToAction("Login", "Account");
				}
			}

			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

			if (user == null) return NotFound();

			var postCount = await _context.Posts.CountAsync(p => p.UserId == user.Id);

			var model = new ProfileViewModel
			{
				UserId = user.Id,
				Nickname = user.Nickname,
				RegistrationDate = user.RegistrationDate,
				LastLoginDate = user.LastLoginDate,
				LoginCount = user.LoginCount,
				PostCount = postCount,
			};

			var currentUserId2 = _userManager.GetUserId(User);
			model.IsOwner = (currentUserId2 == userId);

			return View(model);
		}



		// GET: /Profile/Edit
		[HttpGet]
		public async Task<IActionResult> Edit(string userId)
		{
			var currentUserId = _userManager.GetUserId(User);
			if (userId != currentUserId) return Forbid();

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) return NotFound();

			var model = new EditProfileViewModel
			{
				UserId = user.Id,
				Nickname = user.Nickname,
				AboutMe = user.AboutMe
			};
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditProfileViewModel model)
		{
			var currentUserId = _userManager.GetUserId(User);
			if (model.UserId != currentUserId)
			{
				return Forbid();
			}

			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null)
				return NotFound();

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			user.Nickname = model.Nickname;
			user.AboutMe = model.AboutMe;

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(model);
			}

			return RedirectToAction("Index", new { userId = user.Id });
		}
	}
}
