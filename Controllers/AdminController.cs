using Blog_Zaliczeniowy.Data;
using Blog_Zaliczeniowy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Blog_Zaliczeniowy.Controllers
{
	[Authorize(Roles = "Administrator")]
	public class AdminController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AdminController(ApplicationDbContext context,
						   UserManager<ApplicationUser> userManager,
						   RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Users()
		{
			var users = _userManager.Users.ToList();
			return View(users);
		}

		[HttpGet]
		public async Task<IActionResult> EditUser(string id)
		{
			if (string.IsNullOrEmpty(id)) return NotFound();
			var user = await _userManager.FindByIdAsync(id);
			if (user == null) return NotFound();

			// Przykładowo – pobieramy role przypisane do użytkownika:
			var userRoles = await _userManager.GetRolesAsync(user);

			// Tworzymy model widoku:
			var model = new EditUserViewModel
			{
				UserId = user.Id,
				Nickname = user.Nickname,
				Email = user.Email,
				Roles = userRoles.ToList()
			};

			return View(model);
		}


		[HttpPost]
		public async Task<IActionResult> EditUser(EditUserViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null) return NotFound();

			user.Nickname = model.Nickname;
			user.Email = model.Email; // zmieniasz maila, jeśli chcesz

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(model);
			}

			return RedirectToAction("Users");
		}

		[HttpGet]
		public async Task<IActionResult> Posts()
		{
			var posts = await _context.Posts.Include(p => p.User).ToListAsync();
			return View(posts);
		}

		[HttpGet]
		public async Task<IActionResult> EditPost(int id)
		{
			// Wczytujemy post z bazy danych
			var post = await _context.Posts
				.Include(p => p.User)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (post == null)
			{
				return NotFound();
			}

			// Tworzymy ViewModel na podstawie obiektu z bazy
			var model = new EditPostViewModel
			{
				Id = post.Id,
				Title = post.Title,
				Content = post.Content,
				UserId = post.UserId, // tylko jeśli chcesz edytować również autora
			};

			// (Opcjonalnie) jeśli chcesz wyświetlić listę użytkowników w dropdownie
			ViewBag.Users = new SelectList(_context.Users, "Id", "Nickname", post.UserId);

			return View(model); // Przekazujemy model do widoku
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPost(EditPostViewModel model)
		{
			if (!ModelState.IsValid)
			{
				// Jeśli coś nie tak z walidacją, pokaż formularz ponownie
				return View(model);
			}

			// Wczytujemy post z bazy
			var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == model.Id);
			if (post == null)
			{
				return NotFound();
			}

			// Aktualizujemy dane
			post.Title = model.Title;
			post.Content = model.Content;
			post.UserId = model.UserId;

			// Zapisujemy zmiany w bazie
			_context.Update(post);
			await _context.SaveChangesAsync();

			// Przekierowanie do listy postów w panelu admina (lub do szczegółów posta)
			return RedirectToAction("Posts");
		}

		[HttpGet]
		public async Task<IActionResult> DeletePost(int id)
		{
			var post = await _context.Posts.FindAsync(id);
			if (post != null)
			{
				post.Visibility = false;
				_context.Posts.Update(post);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction("Posts");
		}
		
		[HttpGet]
		public async Task<IActionResult> RestorePost(int id)
		{
			var post = await _context.Posts.FindAsync(id);
			if (post != null)
			{
				post.Visibility = true;
				_context.Posts.Update(post);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction("Posts");
		}

		[HttpGet]
		public async Task<IActionResult> Comments()
		{
			var comments = await _context.Comments
				.Include(c => c.User)
				.Include(c => c.Post)
				.ToListAsync();

			return View(comments);
		}
		[HttpPost]
		public async Task<IActionResult> DeleteComment(int id)
		{
			var comment = await _context.Comments.FindAsync(id);
			if (comment != null)
			{
				_context.Comments.Remove(comment);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction("Comments");
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
