using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Clients;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Client Client { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Client = await _context.Clients.FindAsync(id);
        return Client == null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (_context.Clients.Any(c => c.Email == Client.Email && c.Id != Client.Id))
        {
            ModelState.AddModelError("Client.Email", "Podany adres e-mail już istnieje.");
        }

        if (!ModelState.IsValid) return Page();

        _context.Attach(Client).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}