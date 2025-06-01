using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
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

    public List<Book> Books { get; set; } = [];
    public List<Category> AllCategories { get; set; } = [];

    [BindProperty(SupportsGet = true)] public string? Title { get; set; }
    [BindProperty(SupportsGet = true)] public string? Author { get; set; }
    [BindProperty(SupportsGet = true)] public int? YearFrom { get; set; }
    [BindProperty(SupportsGet = true)] public int? YearTo { get; set; }
    [BindProperty(SupportsGet = true)] public List<int> SelectedCategoryIds { get; set; } = [];
    [BindProperty(SupportsGet = true)] public string? Status { get; set; }
    [BindProperty(SupportsGet = true)] public string? SortBy { get; set; }
    [BindProperty(SupportsGet = true)] public bool SortDesc { get; set; }

    public async Task OnGetAsync()
    {
        AllCategories = await _context.Categories.ToListAsync();

        var query = _context.Books
            .Include(b => b.BookCategories)
            .ThenInclude(bc => bc.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Title))
            query = query.Where(b => b.Title.Contains(Title));

        if (!string.IsNullOrWhiteSpace(Author))
            query = query.Where(b => b.Author.Contains(Author));

        if (Request.Query.ContainsKey("YearFrom") && YearFrom.HasValue)
            query = query.Where(b => b.Year >= YearFrom);

        if (Request.Query.ContainsKey("YearTo") && YearTo.HasValue)
            query = query.Where(b => b.Year <= YearTo);

        if (SelectedCategoryIds.Any())
            query = query.Where(b => b.BookCategories.Any(bc => SelectedCategoryIds.Contains(bc.CategoryId)));

        if (!string.IsNullOrWhiteSpace(Status))
        {
            if (Status == "available")
                query = query.Where(b => !b.IsBorrowed);
            else if (Status == "borrowed")
                query = query.Where(b => b.IsBorrowed);
        }

        query = (SortBy, SortDesc) switch
        {
            ("title", false) => query.OrderBy(b => b.Title),
            ("title", true) => query.OrderByDescending(b => b.Title),
            ("author", false) => query.OrderBy(b => b.Author),
            ("author", true) => query.OrderByDescending(b => b.Author),
            ("year", false) => query.OrderBy(b => b.Year),
            ("year", true) => query.OrderByDescending(b => b.Year),
            _ => query.OrderBy(b => b.Id)
        };

        Books = await query.ToListAsync();
    }
}
