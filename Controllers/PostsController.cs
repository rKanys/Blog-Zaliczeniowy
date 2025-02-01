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
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;

namespace Blog_Zaliczeniowy.Controllers
{
	[Authorize]
	public class PostsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// GET: Posts/Search
		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> Search(string search, int strona = 1)
		{
			var posts = await _context.Posts
				.Include(p => p.User)
				.Where(v => v.Visibility == true)
				.Where(v => v.Approved == true)
				.ToListAsync();
			posts.Reverse();

			if (search != null) return View(paginator(posts, "search", strona, search));
			return View();
		}


		private IEnumerable<Post>? paginator(List<Post> posts, string pagePurpose, int strona = 1, string? search = null)
		{
			IEnumerable<Post>? postCollectionVariable = new List<Post>();
			switch (pagePurpose)
			{
				case "index":
					postCollectionVariable = posts;
					break;
				case "search":
					if (search != null)
					{
						postCollectionVariable = posts.Where(p => p.Title.Contains(search) || p.Content.Contains(search));
					}
					else
					{
						return null;
					}
					break;
			}
			int perStrona = 5;
			int postsCounted = postCollectionVariable.Count();
			int amountToSkip = strona * perStrona - perStrona;
			int countPerStrona = postsCounted / perStrona;
			int toSkip = (amountToSkip < postsCounted ? amountToSkip : postsCounted - (postsCounted % perStrona));

			if (toSkip == postsCounted) toSkip -= 1;

			var newPosts = postCollectionVariable.Skip(toSkip).Take(perStrona);

			ViewBag.TotalPages = (postsCounted % perStrona == 0 ? countPerStrona : countPerStrona + 1);
			ViewBag.PostsFound = postCollectionVariable.Count();
			ViewBag.CurrentPage = (strona <= ViewBag.TotalPages ? strona : ViewBag.TotalPages);
			ViewBag.Search = search;

			return newPosts;
		}

		// GET: Posts
		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> Index(int strona = 1)
		{
			var posts = await _context.Posts
				.Include(p => p.User)
				.Where(v => v.Visibility == true)
				.Where(v => v.Approved == true)
				.ToListAsync();
			posts.Reverse();

			return View(paginator(posts, "index", strona, null));
		}

		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> WaitingRoom(int strona = 1)
		{
			var posts = await _context.Posts
				.Include(p => p.User)
				.Where(v => v.Visibility == true)
				.Where(v => v.Approved == false)
				.ToListAsync();
			posts.Reverse();

			ViewBag.WaitingRoom = true;
			return View(paginator(posts, "index", strona));
		}

		[Authorize(Roles = "Administrator")]
		[HttpGet]
		public async Task<IActionResult> Approve(int id)
		{
			var post = await _context.Posts.FindAsync(id);
			if (post != null)
			{
				post.Approved = true;
				_context.Posts.Update(post);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction("WaitingRoom");
		}

		[AllowAnonymous]
		[HttpGet]
		// GET: Posts/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var post = await _context.Posts
				.Include(p => p.User)
				.Include(p => p.Comments)
				.ThenInclude(c => c.User)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (post == null)
			{
				return NotFound();
			}

			return View(post);
		}

		// GET: Posts/Create
		[Authorize]
		public IActionResult Create()
		{
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Nickname", User.FindFirstValue(ClaimTypes.NameIdentifier));
			return View();
		}


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
						CreatedAt = DateTime.Now,
						UserId = user.Id,
						User = user
					};

					_context.Add(post);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(WaitingRoom));
				}
				else
				{
					// Jeśli użytkownik nie jest zalogowany
					ModelState.AddModelError("", "User not found.");
					return View(postDTO);
				}
			}
			ViewBag.Error = "Zawartość posta musi mieć co najmniej 1 znak i maksymalnie 8000 znaków!";
			return RedirectToAction(nameof(Create));
		}

		// GET: Posts/Edit/5
		public async Task<IActionResult> Edit(int? id)
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
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Nickname", post.UserId);
			return View(post);
		}

		// POST: Post/AddComment/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddComment(int postId, string commentContent, int? parentCommentId)
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

			// Jeśli parentCommentId != null, to znaczy, że jest to odpowiedź na inny komentarz
			var parentComment = parentCommentId.HasValue
				? await _context.Comments.FindAsync(parentCommentId.Value)
				: null;

			// Jeśli nie ma komentarza-rodzica, traktujemy to jako komentarz główny do posta
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
				UserId = user.Id,
				ParentCommentId = parentCommentId
			};

			_context.Comments.Add(comment);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Details), new { id = postId });
		}

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
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Nickname", post.UserId);
			return RedirectToAction(nameof(Index));
		}

		// GET: Posts/Delete/5
		[Authorize]
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
			var _user = _userManager.GetUserId(User);
			if (post.UserId == _user || User.IsInRole("Administrator")) return View(post);
			return RedirectToAction(nameof(Index));
		}

		// POST: Posts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var post = await _context.Posts.FindAsync(id);
			if (post != null)
			{
				var _user = _userManager.GetUserId(User);
				if (post.UserId == _user || User.IsInRole("Administrator"))
				{
					post.Visibility = false;
					_context.Posts.Update(post);
					await _context.SaveChangesAsync();
				}
			}

			return RedirectToAction(nameof(Index));
		}
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		private bool PostExists(int id)
		{
			return _context.Posts.Any(e => e.Id == id);
		}

		[HttpGet]
		public IActionResult Advanced()
		{
			var users = _context.Users
				.Select(u => new { u.Id, u.Nickname })
				.ToList();

			ViewBag.Users = new SelectList(_context.Users, "Id", "Nickname", ""); ;
			return View(new AdvancedSearchViewModel());
		}

		[HttpPost]
		public IActionResult Advanced(AdvancedSearchViewModel model)
		{
			var users = _context.Users
				.Select(u => new { u.Id, u.Nickname })
				.ToList();

			ViewBag.Users = new SelectList(_context.Users, "Id", "Nickname", ""); ;

			var query = _context.Posts
				.Include(p => p.User)
				.AsQueryable();

			// Filtrowanie tytułu (zawiera)
			if (!string.IsNullOrEmpty(model.Title))
			{
				query = query.Where(p => p.Title.Contains(model.Title));
			}

			// Filtrowanie zawartości
			if (!string.IsNullOrEmpty(model.Content))
			{
				query = query.Where(p => p.Content.Contains(model.Content));
			}

			// Filtrowanie daty publikacji
			if (model.FromDate.HasValue)
			{
				query = query.Where(p => p.CreatedAt >= model.FromDate.Value);
			}

			if (model.ToDate.HasValue)
			{
				// Zawężenie wzlędem daty
				var toDate = model.ToDate.Value.AddDays(1).AddSeconds(-1);
				query = query.Where(p => p.CreatedAt <= toDate);
			}

			// Filtrowanie autora
			if (!string.IsNullOrEmpty(model.UserId))
			{
				query = query.Where(p => p.UserId == model.UserId);
			}

			// Agregacje
			// Liczba znalezionych postów
			model.TotalCount = query.Count();

			// Łączna ilość znaków w postach użytkownika
			model.TotalContentLength = query
				.Sum(p => p.Content.Length);

			// Grupowanie po użytkowniku
			var postsByUser = query
				.GroupBy(p => new { p.UserId, p.User.Nickname })
				.Select(g => new PostsByUserAggregate
				{
					UserId = g.Key.UserId,
					Nickname = g.Key.Nickname,
					Count = g.Count(),
					TotalContentLength = g.Sum(x => x.Content.Length)
				})
				.ToList();

			model.PostsByUser = postsByUser;

			model.Results = query
				.OrderByDescending(p => p.CreatedAt)
				.ToList();

			return View(model);
		}
	}
}
