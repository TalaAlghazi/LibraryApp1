using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;

class Program
{
    static IBookRepository repository = new FileBookRepository();
    static ILibraryService libraryService = new LibraryService(repository);

    const int PageSize = 5;

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

            string? choice = Console.ReadLine();

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

    static int AskPageNumber()
    {
        Console.Write("Page number: ");
        return int.TryParse(Console.ReadLine(), out int page) && page > 0 ? page : 1;
    }

    static void DisplayBooks()
    {
        int page = AskPageNumber();
        Console.WriteLine($"\n===== Available Books (page {page}) =====");
        PrintBooks(libraryService.GetAvailableBooks(page, PageSize), "No available books.");
    }

    static void DisplayReservedBooks()
    {
        int page = AskPageNumber();
        Console.WriteLine($"\n===== Reserved Books (page {page}) =====");

        var books = libraryService.GetReservedBooks(page, PageSize);

        if (books.Count == 0)
        {
            Console.WriteLine("No reserved books.");
            return;
        }

        foreach (var book in books)
        {
            string status = book.Fine > 0 ? $"Fine: ${book.Fine}" : "No Fine";
            Console.WriteLine($"{book.Id}. {book.Title} - {book.Author} (Reserved by: {book.BorrowerName}) [{status}]");
        }
    }

    static void ReserveBook()
    {
        Console.Write("Enter Book ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
            return;

        Console.Write("Enter your name: ");
        string? borrowerName = Console.ReadLine();

        Console.WriteLine(libraryService.ReserveBook(id, borrowerName ?? ""));
    }

    static void ReturnBook()
    {
        Console.Write("Enter Book ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
            return;

        Console.WriteLine(libraryService.ReturnBook(id));
    }

    static void SearchBook()
    {
        Console.Write("Enter book title to search: ");
        string? searchTitle = Console.ReadLine();

        int page = AskPageNumber();
        Console.WriteLine($"\n===== Search Results (page {page}) =====");

        var results = libraryService.SearchBook(searchTitle ?? "", page, PageSize);

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
        int page = AskPageNumber();
        Console.WriteLine($"\n===== Outstanding Fines (page {page}) =====");

        var books = libraryService.GetBooksWithFines(page, PageSize);

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