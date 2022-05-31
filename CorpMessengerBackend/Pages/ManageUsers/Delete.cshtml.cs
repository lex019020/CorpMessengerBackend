using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Pages.ManageUsers;

public class DeleteModel : PageModel
{
    private readonly AppDataContext _context;

    public DeleteModel(AppDataContext context)
    {
        _context = context;
    }

    [BindProperty] public User User { get; set; }

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id == null) return NotFound();

        User = await _context.Users
            .Include(u => u.Department).FirstOrDefaultAsync(m => m.UserId == id);

        if (User == null) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(long? id)
    {
        if (id == null) return NotFound();

        User = await _context.Users.FindAsync(id);

        if (User != null)
        {
            User.Deleted = true;
            _context.Users.Update(User);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}