using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public interface ILibraryService
    {
        Result<List<Book>> GetAvailableBooks(int pageNumber, int pageSize);
        Result<List<Book>> SearchBook(string title, int pageNumber, int pageSize);

        Result<List<ReservationWithBookDto>> GetActiveReservations(int pageNumber, int pageSize);
        Result<List<ReservationWithBookDto>> GetReservationsWithFines(int pageNumber, int pageSize);

        Result<string> ReserveBook(int bookId, string borrowerName, string borrowerPhone, int? userId = null);
        Result<string> ReturnBook(int bookId);
        Result<string> UpdateReservation(int reservationId, DateTime newDueDate);
        Result<string> DeleteReservation(int reservationId);

        Result<Book> AddBook(string title, string author, UserRole requesterRole);
        Result<string> DeleteBook(int bookId, UserRole requesterRole);

        Result<List<ReservationWithBookDto>> GetUserReservations(int userId);
        Result<ReservationWithBookDto> GetReservationDetails(int reservationId, int userId, UserRole role);
        Result<string> RequestReturn(int reservationId, int userId);
        Result<List<ReservationWithBookDto>> GetPendingReturns(UserRole role);
        Result<string> ConfirmReturn(int reservationId, UserRole role);

        Result<string> RequestReservation(int bookId, int userId, string borrowerName, string borrowerPhone);
        Result<string> ReserveForCustomer(int bookId, string borrowerName, string borrowerPhone, int? customerUserId, UserRole requesterRole);
        Result<List<ReservationWithBookDto>> GetPendingRequests(UserRole role);
        Result<string> HandOverReservation(int reservationId, UserRole role);
        Result<string> RejectReservation(int reservationId, UserRole role);
    }
}
