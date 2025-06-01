using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Bookly.Data;
using Bookly.Models;

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

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var loan = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Client)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (loan == null)
        {
            return NotFound();
        }

        Loan = loan;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var loanToUpdate = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Client)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (loanToUpdate == null)
        {
            return NotFound();
        }

        if (await TryUpdateModelAsync(
                loanToUpdate,
                "Loan",
                l => l.ReturnDate))
        {
            loanToUpdate.Book.IsBorrowed = false;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        Loan = loanToUpdate;
        return Page();
    }
}