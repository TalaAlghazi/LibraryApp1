using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public interface ILibraryService
    {
        Result<List<Book>> GetAvailableBooks(int pageNumber, int pageSize);
        Result<List<Book>> SearchBook(string title, int pageNumber, int pageSize);

        Result<List<ReservationWithBookDto>> GetActiveReservations(int pageNumber, int pageSize);
        Result<List<ReservationWithBookDto>> GetReservationsWithFines(int pageNumber, int pageSize);

        Result<string> ReserveBook(int bookId, string borrowerName, string borrowerPhone);
        Result<string> ReturnBook(int bookId);
        Result<string> UpdateReservation(int reservationId, DateTime newDueDate);
        Result<string> DeleteReservation(int reservationId);
        Result<Book> AddBook(string title, string author, UserRole requesterRole);
        Result<string> DeleteBook(int bookId, UserRole requesterRole);
    }
}