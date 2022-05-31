using System.Collections.Generic;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Pages.ManageUsers;

public class IndexModel : PageModel
{
    private readonly AppDataContext _context;

    public IndexModel(AppDataContext context)
    {
        _context = context;
    }

    public IList<User> User { get; set; }

    public async Task OnGetAsync()
    {
        User = await _context.Users
            .Include(u => u.Department).ToListAsync();
    }
}