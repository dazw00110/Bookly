using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Categories;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Category Category { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.BookCategories).ThenInclude(bc => bc.Book)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound();

        Category = category;
        return Page();
    }
}