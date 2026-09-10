using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public interface ILibraryService
    {
        Result<List<Book>> GetAvailableBooks(int pageNumber, int pageSize);
        Result<List<Book>> SearchBook(string title, int pageNumber, int pageSize);

        Result<List<Reservation>> GetActiveReservations(int pageNumber, int pageSize);
        Result<List<Reservation>> GetReservationsWithFines(int pageNumber, int pageSize);

        Result<Reservation> ReserveBook(int bookId, string borrowerName);
        Result<Reservation> ReturnBook(int bookId);
    }
}