using System.ComponentModel.DataAnnotations;

namespace Bookly.Models;

public class Client
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Imię jest wymagane.")]
    [RegularExpression("^[A-Za-zÀ-ÖØ-öø-ÿżźćńółęąśŻŹĆĄŚĘŁÓŃ ]{4,}$", ErrorMessage = "Imię musi mieć co najmniej 4 litery i nie może zawierać cyfr.")]
    [Display(Name = "Imię")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nazwisko jest wymagane.")]
    [RegularExpression("^[A-Za-zÀ-ÖØ-öø-ÿżźćńółęąśŻŹĆĄŚĘŁÓŃ ]{4,}$", ErrorMessage = "Nazwisko musi mieć co najmniej 4 litery i nie może zawierać cyfr.")]
    [Display(Name = "Nazwisko")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adres e-mail jest wymagany.")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy adres e-mail.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}