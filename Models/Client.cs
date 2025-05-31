using System.ComponentModel.DataAnnotations;

namespace Bookly.Models;

public class Client
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Imię")]
    public string FirstName { get; set; } = null!;

    [Required]
    [Display(Name = "Nazwisko")]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = null!;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}