using System;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CorpMessengerBackend.Pages.ManageUsers;

public class CreateModel : PageModel
{
    private readonly AppDataContext _context;
    private readonly ICriptographyProvider _cryptographyProvider;

    public CreateModel(AppDataContext context, ICriptographyProvider cryptographyProvider)
    {
        _context = context;
        _cryptographyProvider = cryptographyProvider;
    }

    [BindProperty] public User User { get; set; }

    public string Password { get; set; }

    public IActionResult OnGet()
    {
        ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
        return Page();
    }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync(string password)
    {
        if (!ModelState.IsValid) return Page();

        User.Modified = DateTime.UtcNow;
        var usr = _context.Users.Add(User);
        await _context.SaveChangesAsync();

        _context.UserSecrets.Add(new UserSecret
        {
            UserId = usr.Entity.UserId,
            Secret = _cryptographyProvider.HashPassword(password)
        });

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}