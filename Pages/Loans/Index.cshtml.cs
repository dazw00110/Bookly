using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Loans;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Loan> Loans { get; set; } = [];

    public async Task OnGetAsync()
    {
        Loans = await _context.Loans
            .Include(l => l.Client)
            .Include(l => l.Book)
            .ToListAsync();
    }
}