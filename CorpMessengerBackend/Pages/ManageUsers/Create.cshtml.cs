using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;

namespace CorpMessengerBackend.Pages.ManageUsers
{
    public class CreateModel : PageModel
    {
        private readonly CorpMessengerBackend.Models.AppDataContext _context;

        public CreateModel(CorpMessengerBackend.Models.AppDataContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            return Page();
        }

        [BindProperty]
        public User User { get; set; }
        public String Password { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(String password)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            User.Modified = DateTime.UtcNow;
            var usr = _context.Users.Add(User);
            await _context.SaveChangesAsync();

            _context.UserSecrets.Add(new UserSecret
            {
                UserId = usr.Entity.UserId,
                Secret = CryptographyService.HashPassword(password)
            });

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
