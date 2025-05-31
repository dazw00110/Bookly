using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Loans;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
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

    public async Task<IActionResult> OnPostAsync()
    {
        var loan = await _context.Loans
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.Id == Loan.Id);

        if (loan == null)
        {
            return NotFound();
        }

        loan.ReturnDate = Loan.ReturnDate;

        if (Loan.ReturnDate != null)
        {
            loan.Book.IsBorrowed = false;
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}