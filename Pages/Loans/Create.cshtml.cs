using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Loans;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Loan Loan { get; set; } = new();

    public SelectList Clients { get; set; } = null!;
    public SelectList Books { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadSelectLists();

        // Domyślna data wypożyczenia i planowanego zwrotu
        Loan.LoanDate = DateTime.UtcNow.Date;
        Loan.PlannedReturnDate = DateTime.UtcNow.Date.AddDays(14);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Usuwamy wymagania dla powiązanych obiektów, bo przekazujemy tylko ID
        ModelState.Remove("Loan.Book");
        ModelState.Remove("Loan.Client");

        // Walidacja dat
        if (Loan.LoanDate > DateTime.UtcNow.Date)
        {
            ModelState.AddModelError("Loan.LoanDate", "Data wypożyczenia nie może być w przyszłości.");
        }

        if (Loan.PlannedReturnDate < Loan.LoanDate)
        {
            ModelState.AddModelError("Loan.PlannedReturnDate", "Planowana data zwrotu nie może być wcześniejsza niż data wypożyczenia.");
        }

        // Sprawdzenie dostępności książki
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == Loan.BookId);
        if (book == null)
        {
            ModelState.AddModelError("Loan.BookId", "Wybrana książka nie istnieje.");
        }
        else if (book.IsBorrowed)
        {
            ModelState.AddModelError("Loan.BookId", "Wybrana książka jest już wypożyczona.");
        }

        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return Page();
        }

        // Zapis wypożyczenia i zmiana statusu książki
        book!.IsBorrowed = true;

        Loan.LoanDate = DateTime.SpecifyKind(Loan.LoanDate, DateTimeKind.Utc);
        Loan.PlannedReturnDate = DateTime.SpecifyKind(Loan.PlannedReturnDate, DateTimeKind.Utc);

        _context.Loans.Add(Loan);
        _context.Books.Update(book);

        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    private async Task LoadSelectLists()
    {
        Clients = new SelectList(await _context.Clients.ToListAsync(), "Id", "FullName");
        Books = new SelectList(await _context.Books
            .Where(b => !b.IsBorrowed)
            .ToListAsync(), "Id", "Title");
    }
}
