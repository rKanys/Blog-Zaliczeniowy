using Blog_Zaliczeniowy.Data;
using Blog_Zaliczeniowy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_Zaliczeniowy.Controllers
{
	[Authorize]
	public class MessagesController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// GET: /Messages/Inbox
		public async Task<IActionResult> Inbox()
		{
			var currentUserId = _userManager.GetUserId(User);

			var messages = await _context.PrivateMessages
				.Include(m => m.Sender)
				.Where(m => m.RecipientId == currentUserId)
				.OrderByDescending(m => m.SentAt)
				.ToListAsync();

			return View(messages);
		}

		// GET: /Messages/Sent
		public async Task<IActionResult> Sent()
		{
			var currentUserId = _userManager.GetUserId(User);

			var messages = await _context.PrivateMessages
				.Include(m => m.Recipient)
				.Where(m => m.SenderId == currentUserId)
				.OrderByDescending(m => m.SentAt)
				.ToListAsync();

			return View(messages);
		}

		// GET: /Messages/Details/5
		public async Task<IActionResult> Details(int id)
		{
			var currentUserId = _userManager.GetUserId(User);

			var message = await _context.PrivateMessages
				.Include(m => m.Sender)
				.Include(m => m.Recipient)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (message == null)
			{
				return NotFound();
			}

			if (message.SenderId != currentUserId && message.RecipientId != currentUserId)
			{
				return Forbid();
			}

			if (!message.IsRead && message.RecipientId == currentUserId)
			{
				message.IsRead = true;
				_context.Update(message);
				await _context.SaveChangesAsync();
			}

			return View(message);
		}

		// GET: /Messages/Send?recipientId=...
		// Formularz do wysyłki wiadomości do konkretnego użytkownika
		public IActionResult Send(string recipientId)
		{
			var model = new SendMessageViewModel
			{
				RecipientId = recipientId
			};
			return View(model);
		}

		// POST: /Messages/Send
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Send(SendMessageViewModel model)
		{
			var currentUserId = _userManager.GetUserId(User);
			if (string.IsNullOrEmpty(currentUserId))
			{
				return RedirectToAction("Login", "Account");
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Sprawdź czy odbiorca istnieje
			var recipient = await _userManager.FindByIdAsync(model.RecipientId);
			if (recipient == null)
			{
				ModelState.AddModelError("", "Odbiorca nie istnieje.");
				return View(model);
			}

			// Zapisz wiadomość w bazie
			var message = new PrivateMessage
			{
				SenderId = currentUserId,
				RecipientId = model.RecipientId,
				Content = model.Content,
				SentAt = DateTime.Now,
				IsRead = false
			};

			_context.PrivateMessages.Add(message);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Sent)); // lub np. na Inbox
		}

		// (Opcjonalnie) usuwanie
		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			var currentUserId = _userManager.GetUserId(User);

			var message = await _context.PrivateMessages.FindAsync(id);
			if (message == null)
			{
				return NotFound();
			}

			// Usuwać wiadomość może tylko nadawca lub odbiorca
			if (message.SenderId != currentUserId && message.RecipientId != currentUserId)
			{
				return Forbid();
			}

			_context.PrivateMessages.Remove(message);
			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Inbox));
		}
	}
}
