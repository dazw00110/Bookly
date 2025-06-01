using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Categories;

public class BooksModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public BooksModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public string CategoryName { get; set; } = "";
    public List<Book> Books { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.BookCategories)
            .ThenInclude(bc => bc.Book)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound();

        CategoryName = category.Name;
        Books = category.BookCategories
            .Select(bc => bc.Book)
            .ToList();

        return Page();
    }
}