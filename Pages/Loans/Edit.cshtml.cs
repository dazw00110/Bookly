using System.Diagnostics;
using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public Loan? Loan { get; set; }

    public SelectList Clients { get; set; } = null!;
    public SelectList Books { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Loan = await _context.Loans
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (Loan == null)
            return NotFound();

        await LoadSelectLists();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Remove("Loan.Book");
        ModelState.Remove("Loan.Client");

        Debug.Assert(Loan != null, nameof(Loan) + " != null");
        if (Loan.LoanDate > DateTime.UtcNow.Date)
        {
            ModelState.AddModelError("Loan.LoanDate", "Data wypożyczenia nie może być w przyszłości.");
        }

        if (Loan.PlannedReturnDate < Loan.LoanDate)
        {
            ModelState.AddModelError("Loan.PlannedReturnDate", "Planowana data zwrotu nie może być wcześniejsza niż data wypożyczenia.");
        }

        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return Page();
        }

        var existingLoan = await _context.Loans
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.Id == Loan.Id);

        if (existingLoan == null)
            return NotFound();

        if (existingLoan.BookId != Loan.BookId)
        {
            var oldBook = await _context.Books.FindAsync(existingLoan.BookId);
            var newBook = await _context.Books.FindAsync(Loan.BookId);

            if (newBook == null || newBook.IsBorrowed)
            {
                ModelState.AddModelError("Loan.BookId", "Wybrana książka jest już wypożyczona lub nie istnieje.");
                await LoadSelectLists();
                return Page();
            }

            if (oldBook != null) oldBook.IsBorrowed = false;
            newBook.IsBorrowed = true;

            _context.Books.Update(oldBook!);
            _context.Books.Update(newBook);
        }

        existingLoan.BookId = Loan.BookId;
        existingLoan.ClientId = Loan.ClientId;
        existingLoan.LoanDate = DateTime.SpecifyKind(Loan.LoanDate, DateTimeKind.Utc);
        existingLoan.PlannedReturnDate = DateTime.SpecifyKind(Loan.PlannedReturnDate, DateTimeKind.Utc);

        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }

    private async Task LoadSelectLists()
    {
        Clients = new SelectList(await _context.Clients.ToListAsync(), "Id", "FullName");
        Books = new SelectList(await _context.Books.ToListAsync(), "Id", "Title");
    }
}
