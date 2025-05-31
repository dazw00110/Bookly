using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<BookCategory> BookCategories => Set<BookCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Złożony klucz główny dla tabeli BookCategory
        modelBuilder.Entity<BookCategory>()
            .HasKey(bc => new { bc.BookId, bc.CategoryId });

        // Relacja N:M Book-Category
        modelBuilder.Entity<BookCategory>()
            .HasOne(bc => bc.Book)
            .WithMany(b => b.BookCategories)
            .HasForeignKey(bc => bc.BookId);

        modelBuilder.Entity<BookCategory>()
            .HasOne(bc => bc.Category)
            .WithMany(c => c.BookCategories)
            .HasForeignKey(bc => bc.CategoryId);

        // Book → Loan
        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId);

        // Client → Loan
        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Client)
            .WithMany(c => c.Loans)
            .HasForeignKey(l => l.ClientId);
    }
}