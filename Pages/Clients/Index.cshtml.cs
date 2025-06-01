using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Clients;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Client> Clients { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? SearchText { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? MinLoanCount { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortBy { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool SortDesc { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Clients
            .Include(c => c.Loans)
            .AsQueryable();

        if (!string.IsNullOrEmpty(SearchText))
        {
            query = query.Where(c =>
                c.FirstName.Contains(SearchText) ||
                c.LastName.Contains(SearchText) ||
                c.Email.Contains(SearchText));
        }

        if (MinLoanCount.HasValue)
        {
            query = query.Where(c => c.Loans.Count > MinLoanCount.Value);
        }

        query = (SortBy, SortDesc) switch
        {
            ("first", false) => query.OrderBy(c => c.FirstName),
            ("first", true) => query.OrderByDescending(c => c.FirstName),
            ("last", false) => query.OrderBy(c => c.LastName),
            ("last", true) => query.OrderByDescending(c => c.LastName),
            ("email", false) => query.OrderBy(c => c.Email),
            ("email", true) => query.OrderByDescending(c => c.Email),
            ("count", false) => query.OrderBy(c => c.Loans.Count),
            ("count", true) => query.OrderByDescending(c => c.Loans.Count),
            _ => query.OrderBy(c => c.FirstName)
        };


        Clients = await query.ToListAsync();
    }
}