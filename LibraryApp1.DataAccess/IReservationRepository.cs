namespace LibraryApp1.DataAccess
{
    public interface IReservationRepository
    {
        Reservation? GetById(int id);

        Reservation? GetActiveByBookId(int bookId);

        List<ReservationWithBookDto> GetAllWithBooks(
            int pageNumber,
            int pageSize,
            bool? isActive = null,
            bool? hasFine = null,
            string? borrowerName = null);

        List<ReservationWithBookDto> GetActiveWithBooks(int pageNumber, int pageSize);
        List<ReservationWithBookDto> GetWithFinesAndBooks(int pageNumber, int pageSize);

        List<ReservationWithBookDto> GetByUser(int userId);
        List<ReservationWithBookDto> GetByStatus(ReservationStatus status);
        ReservationWithBookDto? GetDetails(int reservationId);

        List<Reservation> GetAll(
            int pageNumber,
            int pageSize,
            bool? isActive = null,
            bool? hasFine = null,
            string? borrowerName = null);

        void Add(Reservation reservation);
        void Update(Reservation reservation);
        void Delete(int id);

        // Saves a new reservation and the book's availability in one database call.
        // Returns false when the book already has an open reservation.
        bool TryAddWithBook(Reservation reservation, Book book);

        // Saves a reservation change and the book's availability in one database call.
        void UpdateWithBook(Reservation reservation, Book book);
    }
}
