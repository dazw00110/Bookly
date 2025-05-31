using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bookly.Pages.Clients;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Client Client { get; set; } = new();

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        if (_context.Clients.Any(c => c.Email == Client.Email))
        {
            ModelState.AddModelError("Client.Email", "Podany adres e-mail już istnieje.");
        }

        if (!ModelState.IsValid)
            return Page();

        _context.Clients.Add(Client);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}