using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Books;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Book Book { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Book = await _context.Books
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (Book == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var bookToDelete = await _context.Books.FindAsync(Book.Id);
        if (bookToDelete != null)
        {
            _context.Books.Remove(bookToDelete);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("Index");
    }
}