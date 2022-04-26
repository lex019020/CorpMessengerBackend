using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;

namespace CorpMessengerBackend.Pages.ManageUsers
{
    public class EditModel : PageModel
    {
        private readonly CorpMessengerBackend.Models.AppDataContext _context;

        public EditModel(CorpMessengerBackend.Models.AppDataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }
        public String? NewPassword { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User = await _context.Users
                .Include(u => u.Department).FirstOrDefaultAsync(m => m.UserId == id);

            if (User == null)
            {
                return NotFound();
            }
           ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string? newPassword)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            User.Modified = DateTime.UtcNow;

            _context.Attach(User).State = EntityState.Modified;

            if (newPassword != null && newPassword.Length > 7)
            {
                var secret = _context.UserSecrets.First(s => s.UserId == User.UserId);
                if (secret == null)
                {
                    _context.UserSecrets.Add(new UserSecret { 
                        UserId = User.UserId, 
                        Secret = CryptographyService.HashPassword(newPassword) 
                    });
                }
                else
                {
                    secret.Secret = CryptographyService.HashPassword(newPassword);
                    _context.UserSecrets.Update(secret);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
