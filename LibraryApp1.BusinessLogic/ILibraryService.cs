using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public interface ILibraryService
    {
        Result<List<Book>> GetAvailableBooks(int pageNumber, int pageSize);
        Result<List<Book>> SearchBook(string title, int pageNumber, int pageSize);

        Result<List<ReservationWithBookDto>> GetActiveReservations(int pageNumber, int pageSize);
        Result<List<ReservationWithBookDto>> GetReservationsWithFines(int pageNumber, int pageSize);

        Result<Reservation> ReserveBook(int bookId, string borrowerName, string borrowerPhone);
        Result<Reservation> ReturnBook(int bookId);
    }
}