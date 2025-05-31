using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public class Loan
{
    public int Id { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime LoanDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? ReturnDate { get; set; }

    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}