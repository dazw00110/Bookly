using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Author { get; set; } = null!;

    [Required]
    [Range(1000, 2025)]
    public int Year { get; set; }

    public bool IsBorrowed { get; set; } = false;

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}