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

        List<Reservation> GetAll(
            int pageNumber,
            int pageSize,
            bool? isActive = null,
            bool? hasFine = null,
            string? borrowerName = null);

        void Add(Reservation reservation);
        void Update(Reservation reservation);
        void Delete(int id);
    }
}