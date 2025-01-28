using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog_Zaliczeniowy.Data;
using Blog_Zaliczeniowy.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Blog_Zaliczeniowy.Models.DTO.PostDTO;
using System.Collections.Immutable;
using Humanizer;

namespace Blog_Zaliczeniowy.Controllers
{
    public class PostsController : Controller
    {
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// GET: Posts
		public async Task<IActionResult> Index()
		{
			var posts = await _context.Posts.Include(p => p.User).ToListAsync();
			return View(posts);
		}

		// GET: Posts/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var post = await _context.Posts
				.Include(p => p.User)
				.Include(p => p.Comments)  // Wczytanie powiązanych komentarzy
				.ThenInclude(c => c.User)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (post == null)
			{
				return NotFound();
			}

			return View(post);
		}

		// GET: Posts/Create
		public IActionResult Create()
		{
			ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			return View();
		}

		// POST: Posts/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(PostDTO postDTO)
		{
			if (ModelState.IsValid)
			{
				// Pobierz użytkownika zalogowanego
				var user = await _userManager.GetUserAsync(User);

				if (user != null)
				{
					Post post = new Post()
					{
						Title = postDTO.Title,
						Content = postDTO.Content,
						CreatedAt = DateTime.Now,  // Ustaw datę stworzenia posta
						UserId = user.Id,  // Przypisz UserId do posta
						User = user  // Przypisz użytkownika do posta
					};

					_context.Add(post);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));  // Przekierowanie na stronę z postami
				}
				else
				{
					// Jeśli użytkownik nie jest zalogowany
					ModelState.AddModelError("", "User not found.");
					return View(postDTO);
				}
			}
			return View(postDTO);
		}

		// GET: Posts/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

			var post = await _context.Posts
				//.FindAsync(id);
				.Include(p => p.User)
				.FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
            return View(post);
        }

		// POST: Post/AddComment/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddComment(int postId, string commentContent)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				ModelState.AddModelError("", "User is not authenticated");
				return RedirectToAction(nameof(Details), new { id = postId });
			}

			if (string.IsNullOrEmpty(commentContent))
			{
				ModelState.AddModelError("", "Comment content cannot be empty.");
				return RedirectToAction(nameof(Details), new { id = postId });
			}

			var post = await _context.Posts.FindAsync(postId);
			if (post == null)
			{
				return NotFound();
			}

			var comment = new Comment
			{
				Content = commentContent,
				CreatedAt = DateTime.Now,
				PostId = postId,
				UserId = user.Id // Przypisanie UserId (w tym przypadku z zalogowanego użytkownika)
			};

			_context.Comments.Add(comment);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Details), new { id = postId });
		}

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PostDTO postDTO)
        {

			if (!ModelState.IsValid)
			{
				return View(postDTO);
			}
			var post = await _context.Posts
				.Include(p => p.User)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (post == null)
			{
				return NotFound();
			}

			post.Title = postDTO.Title;
			post.Content = postDTO.Content;


			try
			{
				_context.Update(post);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!PostExists(post.Id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
			return RedirectToAction(nameof(Index));
		}

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
