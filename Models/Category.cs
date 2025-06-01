using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookly.Models;

public class Category
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa kategorii jest wymagana.")]
    public string Name { get; set; } = string.Empty;

    public List<BookCategory> BookCategories { get; set; } = new();
}