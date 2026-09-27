using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;

class Program
{
    static LibraryDbContext dbContext = new LibraryDbContext();
    static IBookRepository bookRepository = new SqlBookRepository(dbContext);
    static IReservationRepository reservationRepository = new SqlReservationRepository(dbContext);
    static IUserRepository userRepository = new SqlUserRepository(dbContext);
    static ILibraryService libraryService = new LibraryService(bookRepository, reservationRepository);
    static IAuthService authService = new AuthService(userRepository);

    const int PageSize = 5;

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        while (true)
        {
            var currentUser = RunAuthMenu();
            if (currentUser == null)
                return;

            bool loggedOut = RunMainMenu(currentUser);
            if (!loggedOut)
                return;
        }
    }

    static AuthenticatedUser? RunAuthMenu()
    {
        while (true)
        {
            Console.WriteLine("\n===== Library Management System =====");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Exit");
            Console.Write("Choose: ");

            switch (Console.ReadLine())
            {
                case "1":
                    var loggedIn = Login();
                    if (loggedIn != null) return loggedIn;
                    break;
                case "2":
                    var registered = Register();
                    if (registered != null) return registered;
                    break;
                case "3":
                    return null;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }
    }

    static AuthenticatedUser? Login()
    {
        Console.Write("Username: ");
        string username = Console.ReadLine() ?? "";
        Console.Write("Password: ");
        string password = ReadPassword();

        var result = authService.Login(username, password);
        if (!result.IsSuccess)
        {
            Console.WriteLine($"[{result.Code}] {result.Description}");
            return null;
        }

        Console.WriteLine($"Welcome back, {result.Data!.Username}!");
        return result.Data;
    }

    static AuthenticatedUser? Register()
    {
        Console.Write("Choose a username: ");
        string username = Console.ReadLine() ?? "";
        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";
        Console.Write("Password (min 8 chars, letters + digits): ");
        string password = ReadPassword();
        Console.Write("Confirm password: ");
        string confirmPassword = ReadPassword();

        var result = authService.Register(username, email, password, confirmPassword);
        if (!result.IsSuccess)
        {
            Console.WriteLine($"[{result.Code}] {result.Description}");
            return null;
        }

        Console.WriteLine($"Account created as {result.Data!.Role}. Welcome, {result.Data.Username}!");
        return result.Data;
    }

    static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }

    static bool RunMainMenu(AuthenticatedUser currentUser)
    {
        while (true)
        {
            Console.WriteLine($"\n===== Library ({currentUser.Username} - {currentUser.Role}) =====");
            Console.WriteLine("1. Display Available Books");
            Console.WriteLine("2. Display Active Reservations");
            Console.WriteLine("3. Reserve Book");
            Console.WriteLine("4. Return Book");
            Console.WriteLine("5. Search Book");
            Console.WriteLine("6. View Fines");
            Console.WriteLine("7. Add Book (Admin only)");
            Console.WriteLine("8. Delete Book (Admin only)");
            Console.WriteLine("9. Logout");
            Console.WriteLine("10. Exit");
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
                    AddBook(currentUser);
                    break;
                case "8":
                    DeleteBook(currentUser);
                    break;
                case "9":
                    return true;
                case "10":
                    return false;
                default:
                    Console.WriteLine("Invalid choice.");
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
            Console.WriteLine($"Book {r.BookId}: {r.BookTitle} - reserved by {r.BorrowerName} (due {r.DueDate:d})");
        }
    }

    static void ReserveBook()
    {
        Console.Write("Enter Book ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
            return;

        Console.Write("Enter your name: ");
        string? borrowerName = Console.ReadLine();
        Console.Write("Enter your phone: ");
        string? borrowerPhone = Console.ReadLine();

        var result = libraryService.ReserveBook(id, borrowerName ?? "", borrowerPhone ?? "");

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
            Console.WriteLine($"{r.BorrowerName}: ${r.Fine} ({r.BookTitle})");
            totalFines += r.Fine;
        }

        Console.WriteLine($"\nTotal Fines: ${totalFines}");
    }

    static void AddBook(AuthenticatedUser currentUser)
    {
        Console.Write("Title: ");
        string title = Console.ReadLine() ?? "";
        Console.Write("Author: ");
        string author = Console.ReadLine() ?? "";

        var result = libraryService.AddBook(title, author, currentUser.Role);

        Console.WriteLine(result.IsSuccess
            ? $"Book added successfully (ID {result.Data!.Id})."
            : $"[{result.Code}] {result.Description}");
    }

    static void DeleteBook(AuthenticatedUser currentUser)
    {
        Console.Write("Enter Book ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
            return;

        var result = libraryService.DeleteBook(id, currentUser.Role);

        Console.WriteLine(result.IsSuccess
            ? result.Data
            : $"[{result.Code}] {result.Description}");
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