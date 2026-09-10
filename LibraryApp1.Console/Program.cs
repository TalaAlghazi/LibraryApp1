using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;

class Program
{
    static IBookRepository bookRepository = new FileBookRepository();
    static IReservationRepository reservationRepository = new FileReservationRepository();
    static ILibraryService libraryService = new LibraryService(bookRepository, reservationRepository);

    const int PageSize = 5;

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        while (true)
        {
            Console.WriteLine("\n===== Library =====");
            Console.WriteLine("1. Display Available Books");
            Console.WriteLine("2. Display Active Reservations");
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
                    DisplayAvailableBooks();
                    break;
                case "2":
                    DisplayActiveReservations();
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

    static void DisplayAvailableBooks()
    {
        int page = AskPageNumber();
        Console.WriteLine($"\n===== Available Books (page {page}) =====");

        var result = libraryService.GetAvailableBooks(page, PageSize);

        if (!result.IsSuccess)
        {
            Console.WriteLine($"[{result.Code}] {result.Description}");
            return;
        }

        PrintBooks(result.Data!, "No available books.");
    }

    static void DisplayActiveReservations()
    {
        int page = AskPageNumber();
        Console.WriteLine($"\n===== Active Reservations (page {page}) =====");

        var result = libraryService.GetActiveReservations(page, PageSize);

        if (!result.IsSuccess)
        {
            Console.WriteLine($"[{result.Code}] {result.Description}");
            return;
        }

        var reservations = result.Data!;

        if (reservations.Count == 0)
        {
            Console.WriteLine("No active reservations.");
            return;
        }

        foreach (var r in reservations)
        {
            var book = bookRepository.GetById(r.BookId);
            string title = book?.Title ?? "Unknown";
            Console.WriteLine($"Book {r.BookId}: {title} - reserved by {r.BorrowerName} (due {r.DueDate:d})");
        }
    }

    static void ReserveBook()
    {
        Console.Write("Enter Book ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
            return;

        Console.Write("Enter your name: ");
        string? borrowerName = Console.ReadLine();

        var result = libraryService.ReserveBook(id, borrowerName ?? "");

        Console.WriteLine(result.IsSuccess
            ? result.Description
            : $"[{result.Code}] {result.Description}");
    }

    static void ReturnBook()
    {
        Console.Write("Enter Book ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
            return;

        var result = libraryService.ReturnBook(id);

        Console.WriteLine(result.IsSuccess
            ? result.Description
            : $"[{result.Code}] {result.Description}");
    }

    static void SearchBook()
    {
        Console.Write("Enter book title to search: ");
        string? searchTitle = Console.ReadLine();

        int page = AskPageNumber();
        Console.WriteLine($"\n===== Search Results (page {page}) =====");

        var result = libraryService.SearchBook(searchTitle ?? "", page, PageSize);

        if (!result.IsSuccess)
        {
            Console.WriteLine($"[{result.Code}] {result.Description}");
            return;
        }

        var books = result.Data!;

        if (books.Count == 0)
        {
            Console.WriteLine("No books found.");
            return;
        }

        foreach (var book in books)
        {
            string status = book.IsAvailable ? "Available" : "Reserved";
            Console.WriteLine($"{book.Id}. {book.Title} - {book.Author} ({status})");
        }
    }

    static void ViewFines()
    {
        int page = AskPageNumber();
        Console.WriteLine($"\n===== Outstanding Fines (page {page}) =====");

        var result = libraryService.GetReservationsWithFines(page, PageSize);

        if (!result.IsSuccess)
        {
            Console.WriteLine($"[{result.Code}] {result.Description}");
            return;
        }

        var reservations = result.Data!;

        if (reservations.Count == 0)
        {
            Console.WriteLine("No outstanding fines.");
            return;
        }

        decimal totalFines = 0;
        foreach (var r in reservations)
        {
            var book = bookRepository.GetById(r.BookId);
            string title = book?.Title ?? "Unknown";
            Console.WriteLine($"{r.BorrowerName}: ${r.Fine} ({title})");
            totalFines += r.Fine;
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