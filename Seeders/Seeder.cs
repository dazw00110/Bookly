using Bookly.Data;
using Bookly.Models;

namespace Bookly.Seeders;

public static class Seeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Clients.Any() || context.Books.Any() || context.Categories.Any())
        {
            Console.WriteLine("⚠️ Baza już zawiera dane – seedery pominięte.");
            return;
        }

        var random = new Random();

        // Kategorie
        var categories = new List<Category>
        {
            new() { Name = "Fantasy" },
            new() { Name = "Sci-Fi" },
            new() { Name = "Kryminał" },
            new() { Name = "Romans" },
            new() { Name = "Historyczna" }
        };
        context.Categories.AddRange(categories);

        // Klienci
        var clients = new List<Client>();
        for (int i = 1; i <= 20; i++)
        {
            clients.Add(new Client
            {
                FirstName = $"Imię{i}",
                LastName = $"Nazwisko{i}",
                Email = $"user{i}@mail.com"
            });
        }
        context.Clients.AddRange(clients);
        Console.WriteLine("➡️ Dodano klientów");

        // Książki
        var books = new List<Book>();
        for (int i = 1; i <= 50; i++)
        {
            books.Add(new Book
            {
                Title = $"Książka {i}",
                Author = $"Autor {i}",
                Year = 1980 + random.Next(0, 40)
            });
        }
        context.Books.AddRange(books);
        Console.WriteLine("➡️ Dodano książki");

        // Relacje książka-kategoria (1-2 kategorie na książkę)
        var bookCategories = new List<BookCategory>();
        foreach (var book in books)
        {
            var assigned = new HashSet<int>();
            int categoryCount = random.Next(1, 3);
            for (int i = 0; i < categoryCount; i++)
            {
                int catIndex;
                do
                {
                    catIndex = random.Next(categories.Count);
                } while (!assigned.Add(catIndex));

                bookCategories.Add(new BookCategory
                {
                    Book = book,
                    Category = categories[catIndex]
                });
            }
        }
        context.BookCategories.AddRange(bookCategories);

        // Wypożyczenia (10 losowych)
        var loans = new List<Loan>();
        var usedBooks = new HashSet<int>();
        for (int i = 0; i < 10; i++)
        {
            var book = books[random.Next(books.Count)];
            if (!usedBooks.Add(book.Id)) continue;

            var client = clients[random.Next(clients.Count)];

            loans.Add(new Loan
            {
                Book = book,
                Client = client,
                LoanDate = DateTime.UtcNow.AddDays(-random.Next(1, 14)),
                ReturnDate = random.Next(0, 2) == 0 ? null : DateTime.UtcNow.AddDays(-random.Next(1, 5))
            });

            book.IsBorrowed = true;
        }
        context.Loans.AddRange(loans);
        Console.WriteLine("Dodano relacje i wypożyczenia");

        context.SaveChanges();
        Console.WriteLine("Seeder zakończony pomyślnie!");
    }
}
