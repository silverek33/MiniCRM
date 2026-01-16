using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniCRM.Data;
using MiniCRM.Models;

namespace MiniCRM.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ContactsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Contacts
        public async Task<IActionResult> Index(string? search, string? company, string? sortOrder, int page = 1, int pageSize = 10)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Challenge();

            bool isAdmin = User.IsInRole("Admin");
            IQueryable<Contact> query = _context.Contacts.AsNoTracking();

            if (!isAdmin)
                query = query.Where(c => c.OwnerId == user.Id);

            if (!string.IsNullOrWhiteSpace(company))
                query = query.Where(c => c.Company == company);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c =>
                    EF.Functions.Like(c.LastName, $"%{search}%") ||
                    EF.Functions.Like(c.FirstName, $"%{search}%") ||
                    (c.Email != null && EF.Functions.Like(c.Email, $"%{search}%")));

            query = sortOrder switch
            {
                "lname_desc" => query.OrderByDescending(c => c.LastName).ThenBy(c => c.FirstName),
                "company_asc" => query.OrderBy(c => c.Company).ThenBy(c => c.LastName),
                "company_desc" => query.OrderByDescending(c => c.Company).ThenBy(c => c.LastName),
                _ => query.OrderBy(c => c.LastName).ThenBy(c => c.FirstName)
            };

            int total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var vm = new ContactsIndexViewModel
            {
                Search = search,
                Company = company,
                SortOrder = sortOrder,
                Page = new PagedResult<Contact>
                {
                    Items = items,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalItems = total
                }
            };

            return View(vm);
        }

        // GET: /Contacts/Create
        public IActionResult Create() => View();

        // POST: /Contacts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Phone,Company")] Contact contact)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Challenge();

            // 1) Ustawiamy OwnerId, bo nie przychodzi z formularza
            contact.OwnerId = user.Id;

            // 2) Usuwamy wpis z ModelState, aby Required na OwnerId nie blokował walidacji
            ModelState.Remove(nameof(Contact.OwnerId));

            if (!ModelState.IsValid)
                return View(contact);

            _context.Add(contact);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Kontakt dodany pomyślnie!";
            return RedirectToAction(nameof(Index));
        }


        // GET: /Contacts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact is null) return NotFound();
            if (!CanEdit(contact)) return Forbid();
            return View(contact);
        }

        // POST: /Contacts/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Phone,Company")] Contact input)
        {
            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (contact is null)
            {
                TempData["Error"] = "Nie znaleziono kontaktu do edycji.";
                return RedirectToAction(nameof(Index));
            }
            if (!CanEdit(contact)) return Forbid();

            // ❗ OwnerId jest [Required], ale nie pochodzi z formularza → usuń z ModelState,
            // żeby walidacja nie blokowała zapisu (analogicznie jak w Create).
            ModelState.Remove(nameof(Contact.OwnerId));

            if (!ModelState.IsValid)
            {
                // Pokaż błędy walidacji w widoku
                return View(input);
            }

            try
            {
                contact.FirstName = input.FirstName;
                contact.LastName = input.LastName;
                contact.Email = input.Email;
                contact.Phone = input.Phone;
                contact.Company = input.Company;

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Kontakt „{contact.FirstName} {contact.LastName}” został zaktualizowany.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Wystąpił błąd podczas zapisu zmian. Spróbuj ponownie.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Contacts/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _context.Contacts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (contact is null) return NotFound();
            if (!CanEdit(contact)) return Forbid();
            return View(contact);
        }

        // POST: /Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact is null)
            {
                TempData["Error"] = "Nie znaleziono kontaktu do usunięcia.";
                return RedirectToAction(nameof(Index));
            }
            if (!CanEdit(contact)) return Forbid();

            try
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();

                // ✅ komunikat sukcesu
                TempData["Success"] = $"Kontakt „{contact.FirstName} {contact.LastName}” został usunięty.";
            }
            catch (Exception)
            {
                // ❌ komunikat błędu
                TempData["Error"] = "Nie udało się usunąć kontaktu. Spróbuj ponownie.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CanEdit(Contact c)
        {
            if (User.IsInRole("Admin")) return true;
            var currentUserId = _userManager.GetUserId(User);
            return c.OwnerId == currentUserId;
        }
    }
}