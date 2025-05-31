using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Books;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Book Book { get; set; } = new();

    [BindProperty]
    public List<int> SelectedCategoryIds { get; set; } = new();

    public List<Category> AllCategories { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        Book.Year = DateTime.Now.Year; // 🟢 Domyślny rok (np. 2025)
        AllCategories = await _context.Categories.ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            AllCategories = await _context.Categories.ToListAsync();
            return Page();
        }

        // ➕ Przypisanie kategorii do książki
        Book.BookCategories = SelectedCategoryIds.Select(id => new BookCategory
        {
            CategoryId = id
        }).ToList();

        _context.Books.Add(Book);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}