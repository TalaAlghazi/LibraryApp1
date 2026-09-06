using LibraryApp1.BusinessLogic;

class Program
{
    static LibraryService libraryService = new LibraryService();

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
        var books = libraryService.GetAvailableBooks();

        if (books.Count == 0)
        {
            Console.WriteLine("No available books.");
            return;
        }

        foreach (var book in books)
        {
            Console.WriteLine($"{book.Id}. {book.Title} - {book.Author}");
        }
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
}