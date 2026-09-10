namespace LibraryApp1.DataAccess
{
    public interface IReservationRepository
    {
        Reservation? GetById(int id);

        Reservation? GetActiveByBookId(int bookId);

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
