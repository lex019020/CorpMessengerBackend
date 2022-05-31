using System;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CorpMessengerBackend.Pages.ManageDepartments;

public class CreateModel : PageModel
{
    private readonly AppDataContext _context;

    public CreateModel(AppDataContext context)
    {
        _context = context;
    }

    [BindProperty] public Department Department { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        Department.Modified = DateTime.UtcNow;
        _context.Departments.Add(Department);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}