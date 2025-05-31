using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Books;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Book> Books { get; set; } = new();

    public async Task OnGetAsync()
    {
        Books = await _context.Books
            .Include(b => b.BookCategories)
            .ThenInclude(bc => bc.Category)
            .ToListAsync();
    }
}