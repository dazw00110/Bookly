using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Books;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Book Book { get; set; } = new();

    [BindProperty]
    public List<int> SelectedCategoryIds { get; set; } = new();

    public List<Category> AllCategories { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var book = await _context.Books
            .Include(b => b.BookCategories)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            return NotFound();

        Book = book;
        AllCategories = await _context.Categories.ToListAsync();
        SelectedCategoryIds = book.BookCategories.Select(bc => bc.CategoryId).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            AllCategories = await _context.Categories.ToListAsync();
            return Page();
        }

        var bookToUpdate = await _context.Books
            .Include(b => b.BookCategories)
            .FirstOrDefaultAsync(b => b.Id == Book.Id);

        if (bookToUpdate == null)
            return NotFound();

        bookToUpdate.Title = Book.Title;
        bookToUpdate.Author = Book.Author;
        bookToUpdate.Year = Book.Year;

        bookToUpdate.BookCategories.Clear();

        foreach (var catId in SelectedCategoryIds)
        {
            bookToUpdate.BookCategories.Add(new BookCategory
            {
                BookId = bookToUpdate.Id,
                CategoryId = catId
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}