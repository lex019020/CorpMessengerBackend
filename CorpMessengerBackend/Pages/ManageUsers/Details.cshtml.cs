using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CorpMessengerBackend.Models;

namespace CorpMessengerBackend.Pages.ManageUsers
{
    public class DetailsModel : PageModel
    {
        private readonly CorpMessengerBackend.Models.AppDataContext _context;

        public DetailsModel(CorpMessengerBackend.Models.AppDataContext context)
        {
            _context = context;
        }

        public User User { get; set; }

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
            return Page();
        }
    }
}
