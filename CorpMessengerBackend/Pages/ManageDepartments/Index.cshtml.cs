using System.Collections.Generic;
using System.Threading.Tasks;
using CorpMessengerBackend.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CorpMessengerBackend.Pages.ManageDepartments;

public class IndexModel : PageModel
{
    private readonly AppDataContext _context;

    public IndexModel(AppDataContext context)
    {
        _context = context;
    }

    public IList<Department> Department { get; set; }

    public async Task OnGetAsync()
    {
        Department = await _context.Departments.ToListAsync();
    }
}