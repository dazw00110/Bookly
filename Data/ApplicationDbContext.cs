﻿using Bookly.Models;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Data;

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

        modelBuilder.Entity<BookCategory>()
            .HasKey(bc => new { bc.BookId, bc.CategoryId });

        modelBuilder.Entity<BookCategory>()
            .HasOne(bc => bc.Book)
            .WithMany(b => b.BookCategories)
            .HasForeignKey(bc => bc.BookId);

        modelBuilder.Entity<BookCategory>()
            .HasOne(bc => bc.Category)
            .WithMany(c => c.BookCategories)
            .HasForeignKey(bc => bc.CategoryId);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Client)
            .WithMany(c => c.Loans)
            .HasForeignKey(l => l.ClientId);

        modelBuilder.Entity<Client>()
            .HasIndex(c => c.Email)
            .IsUnique();
    }

}