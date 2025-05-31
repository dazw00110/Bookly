using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Books;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Book Book { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Book = await _context.Books
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Loans).ThenInclude(l => l.Client)
            .FirstOrDefaultAsync(b => b.Id == id);




        if (Book == null)
            return NotFound();

        return Page();
    }
}