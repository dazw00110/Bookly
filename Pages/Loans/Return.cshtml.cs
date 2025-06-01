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

    public Loan? Loan { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            Console.WriteLine("❌ OnGetAsync: ID jest null");
            return NotFound();
        }

        Loan = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Client)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (Loan == null)
        {
            Console.WriteLine($"❌ OnGetAsync: Loan ID={id} nie znaleziony");
            return NotFound();
        }

        Console.WriteLine($"✅ OnGetAsync: Loan załadowany (ID: {id})");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var form = Request.Form;

        if (!int.TryParse(form["loanId"], out int loanId))
        {
            Console.WriteLine("❌ OnPostAsync: Brak loanId w formularzu");
            return NotFound();
        }

        var loan = await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Client)
            .FirstOrDefaultAsync(l => l.Id == loanId);

        if (loan == null)
        {
            Console.WriteLine($"❌ OnPostAsync: Loan ID={loanId} nie znaleziony");
            return NotFound();
        }

        if (!DateTime.TryParse(form["returnDate"], out DateTime returnDate))
        {
            Console.WriteLine("❌ OnPostAsync: Nieprawidłowa data zwrotu");
            ModelState.AddModelError("returnDate", "Nieprawidłowa data.");
            Loan = loan;
            return Page();
        }

        // ✅ Rozwiązanie błędu PostgreSQL - wymuszenie UTC
        returnDate = DateTime.SpecifyKind(returnDate, DateTimeKind.Utc);
        loan.ReturnDate = returnDate;
        loan.Book.IsBorrowed = false;

        try
        {
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ OnPostAsync: Zwrot Loan ID={loanId} zapisany.");
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ OnPostAsync: Błąd zapisu: {ex.Message}");
            ModelState.AddModelError("", "Wystąpił błąd podczas zapisu.");
            Loan = loan;
            return Page();
        }
    }
}
