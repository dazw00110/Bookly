using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Loans;

public class ReturnModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ReturnModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Loan Loan { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Loan = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Client)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (Loan == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var loan = await _context.Loans
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null)
            return NotFound();

        loan.ReturnDate = DateTime.UtcNow;
        loan.Book.IsBorrowed = false;

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}