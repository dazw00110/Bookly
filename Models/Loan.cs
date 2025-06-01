using System.ComponentModel.DataAnnotations;

namespace Bookly.Models;

public class Loan
{
    public int Id { get; set; }

    [Required]
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    [DataType(DataType.Date)]
    public DateTime LoanDate { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime PlannedReturnDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? ReturnDate { get; set; }

    public bool IsOverdue => ReturnDate == null && DateTime.Today > PlannedReturnDate;
}