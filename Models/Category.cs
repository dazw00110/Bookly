using System.ComponentModel.DataAnnotations;

namespace Bookly.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Nazwa kategorii")]
    public string Name { get; set; } = null!;

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}