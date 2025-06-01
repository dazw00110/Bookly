using Bookly.Data;
using Bookly.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Pages.Loans;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Loan> Loans { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchClient { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchBook { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SortBy { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool SortDesc { get; set; }

    public async Task OnGetAsync()
    {
        var query = _context.Loans
            .Include(l => l.Client)
            .Include(l => l.Book)
            .AsQueryable();

        if (!string.IsNullOrEmpty(SearchClient))
        {
            query = query.Where(l =>
                l.Client.FirstName.Contains(SearchClient) ||
                l.Client.LastName.Contains(SearchClient) ||
                l.Client.Email.Contains(SearchClient));
        }

        if (!string.IsNullOrEmpty(SearchBook))
        {
            query = query.Where(l => l.Book.Title.Contains(SearchBook));
        }

        if (!string.IsNullOrEmpty(Status))
        {
            if (Status == "returned")
                query = query.Where(l => l.ReturnDate != null);
            else if (Status == "active")
                query = query.Where(l => l.ReturnDate == null);
        }

        query = (SortBy, SortDesc) switch
        {
            ("book", false) => query.OrderBy(l => l.Book.Title),
            ("book", true) => query.OrderByDescending(l => l.Book.Title),
            ("client", false) => query.OrderBy(l => l.Client.LastName),
            ("client", true) => query.OrderByDescending(l => l.Client.LastName),
            ("loanDate", false) => query.OrderBy(l => l.LoanDate),
            ("loanDate", true) => query.OrderByDescending(l => l.LoanDate),
            ("returnDate", false) => query.OrderBy(l => l.ReturnDate),
            ("returnDate", true) => query.OrderByDescending(l => l.ReturnDate),
            _ => query.OrderBy(l => l.Id)
        };

        Loans = await query.ToListAsync();
    }
}
