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
    public class IndexModel : PageModel
    {
        private readonly CorpMessengerBackend.Models.AppDataContext _context;

        public IndexModel(CorpMessengerBackend.Models.AppDataContext context)
        {
            _context = context;
        }

        public IList<User> User { get;set; }

        public async Task OnGetAsync()
        {
            User = await _context.Users
                .Include(u => u.Department).ToListAsync();
        }
    }
}
