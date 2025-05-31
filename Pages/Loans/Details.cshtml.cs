using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Loans;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Loan Loan { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var loan = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Client)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null)
        {
            return NotFound();
        }

        Loan = loan;
        return Page();
    }
}