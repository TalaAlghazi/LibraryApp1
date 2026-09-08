using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public class LibraryService : ILibraryService
    {
        private readonly IBookRepository repository;

        public LibraryService(IBookRepository bookRepository)
        {
            repository = bookRepository;
        }

        public List<Book> GetAvailableBooks()
            => repository.LoadBooks().FindAll(b => b.IsAvailable);

        public List<Book> GetReservedBooks()
            => repository.LoadBooks().FindAll(b => !b.IsAvailable);

        public List<Book> GetBooksWithFines()
            => repository.LoadBooks().FindAll(b => b.Fine > 0);

        public List<Book> SearchBook(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return new List<Book>();

            return repository.LoadBooks().FindAll(b =>
                b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        public string ReserveBook(int id, string borrowerName)
        {
            if (string.IsNullOrWhiteSpace(borrowerName))
                return "Borrower name cannot be empty.";

            var books = repository.LoadBooks();
            var book = books.Find(b => b.Id == id);

            if (book == null)
                return "Book not found.";

            if (!book.IsAvailable)
                return "Book is already reserved.";

            book.IsAvailable = false;
            book.BorrowerName = borrowerName;
            book.DueDate = DateTime.Now.AddDays(book.GetLoanPeriodDays());
            book.Fine = 0;

            repository.SaveBooks(books);
            return $"Book reserved successfully! Due date: {book.DueDate:d}";
        }

        public string ReturnBook(int id)
        {
            var books = repository.LoadBooks();
            var book = books.Find(b => b.Id == id);

            if (book == null)
                return "Book not found.";

            if (book.IsAvailable)
                return "Book is already available.";

            book.Fine = CalculateFine(book.DueDate, book.GetFinePerDay());
            book.IsAvailable = true;
            book.BorrowerName = "";

            repository.SaveBooks(books);

            return book.Fine > 0
                ? $"Book returned late. Fine: ${book.Fine}"
                : "Book returned on time. No fine.";
        }

        private static decimal CalculateFine(DateTime dueDate, decimal finePerDay)
        {
            int daysLate = (int)(DateTime.Now - dueDate).TotalDays;
            return daysLate > 0 ? daysLate * finePerDay : 0;
        }
    }
}
