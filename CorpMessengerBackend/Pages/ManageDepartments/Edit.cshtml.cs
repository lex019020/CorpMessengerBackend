using System;
using System.Linq;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Pages.ManageDepartments;

public class EditModel : PageModel
{
    private readonly AppDataContext _context;

    public EditModel(AppDataContext context)
    {
        _context = context;
    }

    [BindProperty] public Department Department { get; set; }

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        if (id == null) return NotFound();

        Department = await _context.Departments.FirstOrDefaultAsync(m => m.DepartmentId == id);

        if (Department == null) return NotFound();
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        Department.Modified = DateTime.UtcNow;
        _context.Attach(Department).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DepartmentExists(Department.DepartmentId))
                return NotFound();
            throw;
        }

        return RedirectToPage("./Index");
    }

    private bool DepartmentExists(long id)
    {
        return _context.Departments.Any(e => e.DepartmentId == id);
    }
}