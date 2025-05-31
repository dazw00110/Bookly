using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Clients;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Client Client { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Client = await _context.Clients
            .Include(c => c.Loans)
            .ThenInclude(l => l.Book)
            .FirstOrDefaultAsync(c => c.Id == id);

        return Client == null ? NotFound() : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = await _context.Clients
            .Include(c => c.Loans)
            .ThenInclude(l => l.Book)
            .FirstOrDefaultAsync(c => c.Id == Client.Id);

        if (client != null)
        {
            // Oznacz wszystkie wypożyczone książki jako dostępne
            foreach (var loan in client.Loans)
            {
                if (loan.Book != null)
                    loan.Book.IsBorrowed = false;

                _context.Loans.Remove(loan);
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("Index");
    }
}