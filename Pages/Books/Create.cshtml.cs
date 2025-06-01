using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Books
{
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

        public string? CategoryValidationError { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            AllCategories = await _context.Categories.ToListAsync();

            // ➤ Ustawienie domyślnego roku na bieżący (np. 2025)
            if (Book.Year == 0)
            {
                Book.Year = DateTime.Now.Year;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AllCategories = await _context.Categories.ToListAsync();

            if (SelectedCategoryIds == null || !SelectedCategoryIds.Any())
            {
                CategoryValidationError = "Musisz wybrać przynajmniej jedną kategorię.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            foreach (var categoryId in SelectedCategoryIds)
            {
                Book.BookCategories.Add(new BookCategory
                {
                    CategoryId = categoryId,
                    Book = Book
                });
            }

            _context.Books.Add(Book);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}