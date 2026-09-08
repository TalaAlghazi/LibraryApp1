using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public interface ILibraryService
    {
        List<Book> GetAvailableBooks(int pageNumber, int pageSize);
        List<Book> GetReservedBooks(int pageNumber, int pageSize);
        List<Book> GetBooksWithFines(int pageNumber, int pageSize);
        List<Book> SearchBook(string title, int pageNumber, int pageSize);
        string ReserveBook(int id, string borrowerName);
        string ReturnBook(int id);
    }
}
