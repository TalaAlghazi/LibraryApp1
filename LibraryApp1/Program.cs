using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public bool IsAvailable { get; set; }
    public string BorrowerName { get; set; } = "";
    public DateTime DueDate { get; set; }
    public decimal Fine { get; set; } = 0;
}

class Program
{
    static List<Book> books = new List<Book>();
    static string filePath = "books.json";
    static decimal FINE_PER_DAY = 1; // $1 per day

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        LoadBooks();

        while (true)
        {
            Console.WriteLine("\n===== Library =====");
            Console.WriteLine("1. Display Books");
            Console.WriteLine("2. Display Reserved Books");
            Console.WriteLine("3. Reserve Book");
            Console.WriteLine("4. Return Book");
            Console.WriteLine("5. Search Book");
            Console.WriteLine("6. View Fines");
            Console.WriteLine("7. Exit");
            Console.Write("Choose: ");

            string choice = Console.ReadLine();

            if (choice == "1")
                DisplayBooks();
            else if (choice == "2")
                DisplayReservedBooks();
            else if (choice == "3")
                ReserveBook();
            else if (choice == "4")
                ReturnBook();
            else if (choice == "5")
                SearchBook();
            else if (choice == "6")
                ViewFines();
            else if (choice == "7")
                break;
        }
    }

    static void DisplayBooks()
    {
        Console.WriteLine("\n===== Available Books =====");
        var availableBooks = books.FindAll(b => b.IsAvailable);

        if (availableBooks.Count == 0)
        {
            Console.WriteLine("No available books.");
            return;
        }

        foreach (var book in availableBooks)
        {
            Console.WriteLine($"{book.Id}. {book.Title} - {book.Author}");
        }
    }

    static void DisplayReservedBooks()
    {
        Console.WriteLine("\n===== Reserved Books =====");
        var reservedBooks = books.FindAll(b => !b.IsAvailable);

        if (reservedBooks.Count == 0)
        {
            Console.WriteLine("No reserved books.");
            return;
        }

        foreach (var book in reservedBooks)
        {
            string status = book.Fine > 0 ? $"Fine: ${book.Fine}" : "No Fine";
            Console.WriteLine($"{book.Id}. {book.Title} - {book.Author} (Reserved by: {book.BorrowerName}, Due: {book.DueDate.ToShortDateString()}, {status})");
        }
    }

    static void ReserveBook()
    {
        Console.Write("Enter Book ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var book = books.Find(b => b.Id == id);
            if (book != null && book.IsAvailable)
            {
                Console.Write("Enter your name: ");
                string borrowerName = Console.ReadLine();

                book.IsAvailable = false;
                book.BorrowerName = borrowerName;
                book.DueDate = DateTime.Now.AddDays(14);
                book.Fine = 0;
                SaveBooks();
                Console.WriteLine($"Book reserved successfully! Due date: {book.DueDate.ToShortDateString()}");
            }
            else
            {
                Console.WriteLine("Book not found or already reserved.");
            }
        }
    }

    static void ReturnBook()
    {
        Console.Write("Enter Book ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var book = books.Find(b => b.Id == id);
            if (book != null && !book.IsAvailable)
            {
                // Calculate fine
                int daysLate = (int)(DateTime.Now - book.DueDate).TotalDays;
                if (daysLate > 0)
                {
                    book.Fine = daysLate * FINE_PER_DAY;
                    Console.WriteLine($"Book is {daysLate} days late. Fine: ${book.Fine}");
                }
                else
                {
                    Console.WriteLine("Book returned on time! No fine.");
                }

                book.IsAvailable = true;
                book.BorrowerName = "";
                SaveBooks();
                Console.WriteLine("Book returned successfully!");
            }
            else
            {
                Console.WriteLine("Book not found or already available.");
            }
        }
    }

    static void SearchBook()
    {
        Console.Write("Enter book title to search: ");
        string searchTitle = Console.ReadLine().ToLower();

        Console.WriteLine("\n===== Search Results =====");
        var results = books.FindAll(b => b.Title.ToLower().Contains(searchTitle));

        if (results.Count == 0)
        {
            Console.WriteLine("No books found.");
            return;
        }

        foreach (var book in results)
        {
            string status = book.IsAvailable ? "Available" : "Reserved";
            Console.WriteLine($"{book.Id}. {book.Title} - {book.Author} ({status})");
        }
    }

    static void ViewFines()
    {
        Console.WriteLine("\n===== Outstanding Fines =====");
        var booksWithFines = books.FindAll(b => b.Fine > 0);

        if (booksWithFines.Count == 0)
        {
            Console.WriteLine("No outstanding fines.");
            return;
        }

        decimal totalFines = 0;
        foreach (var book in booksWithFines)
        {
            Console.WriteLine($"{book.BorrowerName}: ${book.Fine} ({book.Title})");
            totalFines += book.Fine;
        }
        Console.WriteLine($"\nTotal Fines: ${totalFines}");
    }

    static void LoadBooks()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            books = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }
        else
        {
            books = new List<Book>
            {
                new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", IsAvailable = true },
                new Book { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee", IsAvailable = true },
                new Book { Id = 3, Title = "1984", Author = "George Orwell", IsAvailable = true },
                new Book { Id = 4, Title = "Pride and Prejudice", Author = "Jane Austen", IsAvailable = true },
                new Book { Id = 5, Title = "The Catcher in the Rye", Author = "J.D. Salinger", IsAvailable = true },
                new Book { Id = 6, Title = "Brave New World", Author = "Aldous Huxley", IsAvailable = true },
                new Book { Id = 7, Title = "Jane Eyre", Author = "Charlotte Bronte", IsAvailable = true },
                new Book { Id = 8, Title = "Wuthering Heights", Author = "Emily Bronte", IsAvailable = true }
            };
            SaveBooks();
        }
    }

    static void SaveBooks()
    {
        string json = JsonSerializer.Serialize(books);
        File.WriteAllText(filePath, json);
    }
}
