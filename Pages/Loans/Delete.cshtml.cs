using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Loans;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
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

        if (loan != null)
        {
            // Jeśli nie był zwrócony, zwróć książkę przed usunięciem wypożyczenia
            if (loan.ReturnDate == null)
            {
                loan.Book.IsBorrowed = false;
            }

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("Index");
    }
}