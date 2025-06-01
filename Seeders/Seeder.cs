using Bookly.Data;
using Bookly.Models;
using Bogus;

namespace Bookly.Seeders;

public static class Seeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Clients.Any() || context.Books.Any() || context.Categories.Any())
        {
            Console.WriteLine("📦 Baza danych już zawiera dane – pomijam seedowanie.");
            return;
        }

        var faker = new Faker("pl");
        var random = new Random();

        // 🔹 Kategorie
        var categoryNames = new[]
        {
            "Fantasy", "Science Fiction", "Kryminał", "Romans", "Historyczna",
            "Literatura faktu", "Dziecięca", "Thriller", "Psychologia", "Technologia"
        };

        var categories = categoryNames.Select((name, i) => new Category
        {
            Id = i + 1,
            Name = name
        }).ToList();
        context.Categories.AddRange(categories);

        // 🔹 Klienci
        var clients = new Faker<Client>("pl")
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
            .Generate(50);
        context.Clients.AddRange(clients);

        // 🔹 Książki
        var books = new Faker<Book>("pl")
            .RuleFor(b => b.Title, f => f.Lorem.Sentence(3))
            .RuleFor(b => b.Author, f => f.Name.FullName())
            .RuleFor(b => b.Year, f => f.Random.Int(1950, 2024))
            .RuleFor(b => b.IsBorrowed, false)
            .Generate(200);
        context.Books.AddRange(books);

        context.SaveChanges(); // 🔑 TERAZ ID są dostępne

        // 🔹 BookCategories
        var bookCategories = new List<BookCategory>();
        foreach (var book in books)
        {
            var count = random.Next(1, 4);
            var pickedCategories = categories.OrderBy(_ => random.Next()).Take(count).ToList();
            foreach (var cat in pickedCategories)
            {
                bookCategories.Add(new BookCategory
                {
                    BookId = book.Id,
                    CategoryId = cat.Id
                });
            }
        }
        context.BookCategories.AddRange(bookCategories);

        // 🔹 Wypożyczenia
        var loans = new List<Loan>();
        var usedBookIds = new HashSet<int>();
        var savedClients = context.Clients.ToList();
        var savedBooks = context.Books.ToList();

        for (int i = 0; i < 30; i++)
        {
            var client = savedClients[random.Next(savedClients.Count)];
            var availableBooks = savedBooks.Where(b => !usedBookIds.Contains(b.Id)).ToList();
            if (!availableBooks.Any()) break;

            var book = availableBooks[random.Next(availableBooks.Count)];
            usedBookIds.Add(book.Id);

            var loanDate = faker.Date.Past(2).ToUniversalTime();
            var plannedReturn = loanDate.AddDays(14);
            DateTime? returnDate = null;

            if (random.NextDouble() < 0.7)
            {
                returnDate = loanDate.AddDays(random.Next(1, 15));
                book.IsBorrowed = false;
            }
            else
            {
                book.IsBorrowed = true;
            }

            loans.Add(new Loan
            {
                ClientId = client.Id,
                BookId = book.Id,
                LoanDate = loanDate,
                PlannedReturnDate = plannedReturn,
                ReturnDate = returnDate
            });
        }

        context.Loans.AddRange(loans);
        context.SaveChanges();

        Console.WriteLine($"✅ Dodano: {categories.Count} kategorii, {clients.Count} klientów, {books.Count} książek, {loans.Count} wypożyczeń");
    }
}
