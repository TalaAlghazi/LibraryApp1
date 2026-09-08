using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;

class Program
{
    static IBookRepository repository = new FileBookRepository();
    static ILibraryService libraryService = new LibraryService(repository);

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

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

            switch (choice)
            {
                case "1":
                    DisplayBooks();
                    break;
                case "2":
                    DisplayReservedBooks();
                    break;
                case "3":
                    ReserveBook();
                    break;
                case "4":
                    ReturnBook();
                    break;
                case "5":
                    SearchBook();
                    break;
                case "6":
                    ViewFines();
                    break;
                case "7":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static void DisplayBooks()
    {
        Console.WriteLine("\n===== Available Books =====");
        PrintBooks(libraryService.GetAvailableBooks(), "No available books.");
    }

    static void DisplayReservedBooks()
    {
        Console.WriteLine("\n===== Reserved Books =====");
        var books = libraryService.GetReservedBooks();

        if (books.Count == 0)
        {
            Console.WriteLine("No reserved books.");
            return;
        }

        foreach (var book in books)
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
            Console.Write("Enter your name: ");
            string borrowerName = Console.ReadLine();

            string result = libraryService.ReserveBook(id, borrowerName);
            Console.WriteLine(result);
        }
    }

    static void ReturnBook()
    {
        Console.Write("Enter Book ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            string result = libraryService.ReturnBook(id);
            Console.WriteLine(result);
        }
    }

    static void SearchBook()
    {
        Console.Write("Enter book title to search: ");
        string searchTitle = Console.ReadLine();

        Console.WriteLine("\n===== Search Results =====");
        var results = libraryService.SearchBook(searchTitle);

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
        var books = libraryService.GetBooksWithFines();

        if (books.Count == 0)
        {
            Console.WriteLine("No outstanding fines.");
            return;
        }

        decimal totalFines = 0;
        foreach (var book in books)
        {
            Console.WriteLine($"{book.BorrowerName}: ${book.Fine} ({book.Title})");
            totalFines += book.Fine;
        }
        Console.WriteLine($"\nTotal Fines: ${totalFines}");
    }
        static void PrintBook(Book book)
        {
            Console.WriteLine($"{book.Id}. {book.Title} - {book.Author}");
        }

        static void PrintBooks(List<Book> books, string emptyMessage)
        {
            if (books.Count == 0)
            {
                Console.WriteLine(emptyMessage);
                return;
            }

            foreach (var book in books)
                PrintBook(book);
        }
    }
