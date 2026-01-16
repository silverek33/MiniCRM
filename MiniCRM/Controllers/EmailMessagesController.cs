using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCRM.Data;
using MiniCRM.Models;
using MiniCRM.Services;

namespace MiniCRM.Controllers
{
    [Authorize]
    public class EmailMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailService _emailService;

        public EmailMessagesController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: /EmailMessages/ForContact/5
        public async Task<IActionResult> ForContact(int contactId)
        {
            var contact = await _context.Contacts
                .Include(c => c.EmailMessages)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == contactId);

            if (contact is null) return NotFound();

            if (!User.IsInRole("Admin") && contact.OwnerId != _userManager.GetUserId(User))
                return Forbid();

            return View(contact);
        }

        // GET: /EmailMessages/Create?contactId=5
        public async Task<IActionResult> Create(int contactId)
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact is null) return NotFound();

            if (!User.IsInRole("Admin") && contact.OwnerId != _userManager.GetUserId(User))
                return Forbid();

            var model = new EmailMessage
            {
                ContactId = contactId,
                To = contact.Email ?? string.Empty
            };
            return View(model);
        }

        // POST: /EmailMessages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContactId,To,Subject,Body")] EmailMessage message)
        {
            var contact = await _context.Contacts.FindAsync(message.ContactId);
            if (contact is null) return NotFound();

            if (!User.IsInRole("Admin") && contact.OwnerId != _userManager.GetUserId(User))
                return Forbid();

            if (!ModelState.IsValid) return View(message);

            _context.EmailMessages.Add(message);
            await _context.SaveChangesAsync();

            try
            {
                await _emailService.SendAsync(message.To, message.Subject, message.Body);
                message.Sent = true;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Wiadomość e‑mail została utworzona i wysłana.";
            }
            catch
            {
                // Rekord już istnieje w bazie, ale nie udało się wysłać:
                message.Sent = false;
                await _context.SaveChangesAsync();

                TempData["Warning"] = "Wiadomość została utworzona, ale nie udało się jej wysłać. Spróbuj ponownie później.";
                // Opcjonalnie: ModelState.AddModelError("", "Nie udało się wysłać e‑maila.");
            }

            return RedirectToAction(nameof(ForContact), new { contactId = message.ContactId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var msg = await _context.EmailMessages.FindAsync(id);
            if (msg is null)
            {
                TempData["Error"] = "Nie znaleziono wiadomości do usunięcia.";
                return RedirectToAction("Index", "Contacts");
            }

            // autoryzacja: admin wszystko, user tylko swoje (po kontakcie)
            var contact = await _context.Contacts.FindAsync(msg.ContactId);
            if (contact is null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (!User.IsInRole("Admin") && contact.OwnerId != currentUserId)
                return Forbid();

            try
            {
                _context.EmailMessages.Remove(msg);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Wiadomość została usunięta.";
            }
            catch
            {
                TempData["Error"] = "Nie udało się usunąć wiadomości.";
            }

            return RedirectToAction(nameof(ForContact), new { contactId = msg.ContactId });
        }
    }
}