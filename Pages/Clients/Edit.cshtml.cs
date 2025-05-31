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
    public Client Client { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
        {
            return NotFound();
        }

        Client = client;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Sprawdzenie unikalności e-maila (opcjonalnie)
        var existing = await _context.Clients
            .Where(c => c.Id != Client.Id)
            .AnyAsync(c => c.Email == Client.Email);

        if (existing)
        {
            ModelState.AddModelError("Client.Email", "Podany adres e-mail jest już używany.");
            return Page();
        }

        _context.Attach(Client).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Clients.AnyAsync(e => e.Id == Client.Id))
            {
                return NotFound();
            }
            throw;
        }

        return RedirectToPage("Index");
    }
}