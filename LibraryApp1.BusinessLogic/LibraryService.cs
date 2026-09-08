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

        public List<Book> GetAvailableBooks(int pageNumber, int pageSize)
            => repository.GetAll(pageNumber, pageSize, isAvailable: true);

        public List<Book> GetReservedBooks(int pageNumber, int pageSize)
            => repository.GetAll(pageNumber, pageSize, isAvailable: false);

        public List<Book> GetBooksWithFines(int pageNumber, int pageSize)
            => repository.GetAll(pageNumber, pageSize, hasFine: true);

        public List<Book> SearchBook(string title, int pageNumber, int pageSize)
            => repository.GetAll(pageNumber, pageSize, titleContains: title);

        public string ReserveBook(int id, string borrowerName)
        {
            if (string.IsNullOrWhiteSpace(borrowerName))
                return "Borrower name cannot be empty.";

            var book = repository.GetById(id);

            if (book == null)
                return "Book not found.";

            if (!book.IsAvailable)
                return "Book is already reserved.";

            book.IsAvailable = false;
            book.BorrowerName = borrowerName;
            book.DueDate = DateTime.Now.AddDays(book.GetLoanPeriodDays());
            book.Fine = 0;

            repository.Update(book);
            return $"Book reserved successfully! Due date: {book.DueDate:d}";
        }

        public string ReturnBook(int id)
        {
            var book = repository.GetById(id);

            if (book == null)
                return "Book not found.";

            if (book.IsAvailable)
                return "Book is already available.";

            book.Fine = CalculateFine(book.DueDate, book.GetFinePerDay());
            book.IsAvailable = true;
            book.BorrowerName = "";

            repository.Update(book);

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