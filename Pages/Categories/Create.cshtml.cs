using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Categories;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Category Category { get; set; } = new();

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Sprawdzenie czy taka nazwa już istnieje
        var exists = await _context.Categories.AnyAsync(c => c.Name == Category.Name);
        if (exists)
        {
            ModelState.AddModelError("Category.Name", "Taka kategoria już istnieje.");
            return Page();
        }

        try
        {
            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
            ModelState.AddModelError(string.Empty, "Wystąpił błąd przy zapisie. Możliwe, że ID się powtarza.");
            return Page();
        }

        return RedirectToPage("Index");
    }
}