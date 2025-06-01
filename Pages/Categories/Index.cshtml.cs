using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Categories;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Category> Categories { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? SearchName { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortBy { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool SortDesc { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Categories
            .Include(c => c.BookCategories)
            .ThenInclude(bc => bc.Book)
            .AsQueryable();

        if (!string.IsNullOrEmpty(SearchName))
        {
            query = query.Where(c => c.Name.Contains(SearchName));
        }

        query = (SortBy, SortDesc) switch
        {
            ("name", false) => query.OrderBy(c => c.Name),
            ("name", true) => query.OrderByDescending(c => c.Name),
            ("count", false) => query.OrderBy(c => c.BookCategories.Count),
            ("count", true) => query.OrderByDescending(c => c.BookCategories.Count),
            _ => query.OrderBy(c => c.Name)
        };


        Categories = await query.ToListAsync();
    }
}