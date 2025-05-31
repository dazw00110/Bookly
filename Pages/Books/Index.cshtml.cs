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

    public List<Book> Books { get; set; } = new();
    public List<Category> AllCategories { get; set; } = new();

    [BindProperty(SupportsGet = true)] public string? Title { get; set; }
    [BindProperty(SupportsGet = true)] public string? Author { get; set; }
    [BindProperty(SupportsGet = true)] public int? YearFrom { get; set; }
    [BindProperty(SupportsGet = true)] public int? YearTo { get; set; }
    [BindProperty(SupportsGet = true)] public List<int> SelectedCategoryIds { get; set; } = new();
    [BindProperty(SupportsGet = true)] public string? Status { get; set; }

    public async Task OnGetAsync()
    {
        AllCategories = await _context.Categories.ToListAsync();

        // 🟢 Ustawienie domyślnych wartości do formularza (ale nie używane w filtrach)
        if (YearFrom == null) YearFrom = DateTime.Now.Year;
        if (YearTo == null) YearTo = DateTime.Now.Year;

        var query = _context.Books
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(Title))
            query = query.Where(b => b.Title.Contains(Title));

        if (!string.IsNullOrWhiteSpace(Author))
            query = query.Where(b => b.Author.Contains(Author));

        // ✅ Tylko jeśli użytkownik faktycznie wysłał te pola
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

        Books = await query.ToListAsync();
    }
}
