using System.ComponentModel.DataAnnotations;

namespace Bookly.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tytuł jest wymagany.")]
    [MinLength(3, ErrorMessage = "Tytuł musi mieć co najmniej 3 znaki.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Autor jest wymagany.")]
    [MinLength(4, ErrorMessage = "Autor musi mieć co najmniej 4 znaki.")]
    [RegularExpression(@"^[A-Za-zżźćńółęąśŻŹĆĄŚĘŁÓŃ\s]+$", ErrorMessage = "Autor może zawierać tylko litery i spacje.")]
    public string Author { get; set; } = null!;

    [Required(ErrorMessage = "Rok wydania jest wymagany.")]
    [Range(0, 2025, ErrorMessage = "Rok wydania musi być w zakresie od 0 do 2025.")]
    public int Year { get; set; }

    public bool IsBorrowed { get; set; } = false;

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    [Display(Name = "Tytuł - Autor")]
    public string TitleAndAuthor => $"{Title} - {Author}";
}