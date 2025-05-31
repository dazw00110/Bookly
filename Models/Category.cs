using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}