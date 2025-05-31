using System.ComponentModel.DataAnnotations;

namespace Bookly.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tytuł jest wymagany.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Autor jest wymagany.")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rok jest wymagany.")]
    [Range(1, 2025, ErrorMessage = "Rok musi być z przedziału 1–2025.")]
    public int Year { get; set; }

    public bool IsBorrowed { get; set; } = false;

    public List<BookCategory> BookCategories { get; set; } = new();
    public List<Loan> Loans { get; set; } = new();
}