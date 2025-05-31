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
        Clients = new SelectList(await _context.Clients.ToListAsync(), "Id", "Email");
        Books = new SelectList(await _context.Books.Where(b => !b.IsBorrowed).ToListAsync(), "Id", "Title");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var book = await _context.Books.FindAsync(Loan.BookId);
        if (book == null || book.IsBorrowed)
        {
            ModelState.AddModelError("", "Wybrana książka jest już wypożyczona.");
            return Page();
        }

        Loan.LoanDate = DateTime.UtcNow.Date; // ✅ Ustawienie daty w UTC

        _context.Loans.Add(Loan);
        book.IsBorrowed = true;

        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}