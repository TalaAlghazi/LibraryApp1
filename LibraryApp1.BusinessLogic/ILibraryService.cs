using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public interface ILibraryService
    {
        List<Book> GetAvailableBooks();
        List<Book> GetReservedBooks();
        List<Book> GetBooksWithFines();
        List<Book> SearchBook(string title);
        string ReserveBook(int id, string borrowerName);
        string ReturnBook(int id);
    }
}
