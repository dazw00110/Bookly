using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Clients;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Client? Client { get; set; }
    public List<Loan> Loans { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Client = await _context.Clients.FindAsync(id);
        if (Client == null) return NotFound();

        Loans = await _context.Loans
            .Include(l => l.Book)
            .Where(l => l.ClientId == id)
            .ToListAsync();

        return Page();
    }
}