using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public class LibraryService
    {
        private BookRepository repository = new BookRepository();
        private List<Book> books;
        private decimal FINE_PER_DAY = 1;

        public LibraryService()
        {
            books = repository.LoadBooks();
        }

        public List<Book> GetAvailableBooks()
        {
            return books.FindAll(b => b.IsAvailable);
        }

        public List<Book> GetReservedBooks()
        {
            return books.FindAll(b => !b.IsAvailable);
        }

        public string ReserveBook(int id, string borrowerName)
        {
            var book = books.Find(b => b.Id == id);
            if (book != null && book.IsAvailable)
            {
                book.IsAvailable = false;
                book.BorrowerName = borrowerName;
                book.DueDate = DateTime.Now.AddDays(14);
                book.Fine = 0;
                repository.SaveBooks(books);
                return $"Book reserved successfully! Due date: {book.DueDate.ToShortDateString()}";
            }
            return "Book not found or already reserved.";
        }

        public string ReturnBook(int id)
        {
            var book = books.Find(b => b.Id == id);
            if (book != null && !book.IsAvailable)
            {
                int daysLate = (int)(DateTime.Now - book.DueDate).TotalDays;
                string message;

                if (daysLate > 0)
                {
                    book.Fine = daysLate * FINE_PER_DAY;
                    message = $"Book is {daysLate} days late. Fine: ${book.Fine}\n";
                }
                else
                {
                    message = "Book returned on time! No fine.\n";
                }

                book.IsAvailable = true;
                book.BorrowerName = "";
                repository.SaveBooks(books);
                return message + "Book returned successfully!";
            }
            return "Book not found or already available.";
        }

        public List<Book> SearchBook(string searchTitle)
        {
            return books.FindAll(b => b.Title.ToLower().Contains(searchTitle.ToLower()));
        }

        public List<Book> GetBooksWithFines()
        {
            return books.FindAll(b => b.Fine > 0);
        }
    }
}
