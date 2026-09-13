namespace LibraryApp1.DataAccess
{
    public class SqlReservationRepository : IReservationRepository
    {
        private readonly LibraryDbContext context;

        public SqlReservationRepository(LibraryDbContext context)
        {
            this.context = context;
        }

        public Reservation? GetById(int id)
        {
            return context.Reservations.Find(id);
        }

        public Reservation? GetActiveByBookId(int bookId)
        {
            return context.Reservations
                .FirstOrDefault(r => r.BookId == bookId && r.ReturnedAt == null);
        }

        public List<Reservation> GetAll(
            int pageNumber,
            int pageSize,
            bool? isActive = null,
            bool? hasFine = null,
            string? borrowerName = null)
        {
            IQueryable<Reservation> query = context.Reservations;

            if (isActive.HasValue)
                query = isActive.Value
                    ? query.Where(r => r.ReturnedAt == null)
                    : query.Where(r => r.ReturnedAt != null);

            if (hasFine.HasValue)
                query = hasFine.Value
                    ? query.Where(r => r.Fine > 0)
                    : query.Where(r => r.Fine == 0);

            if (!string.IsNullOrWhiteSpace(borrowerName))
                query = query.Where(r => r.BorrowerName.Contains(borrowerName));

            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public void Add(Reservation reservation)
        {
            context.Reservations.Add(reservation);
            context.SaveChanges();
        }

        public void Update(Reservation reservation)
        {
            context.Reservations.Update(reservation);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var reservation = context.Reservations.Find(id);
            if (reservation == null) return;

            context.Reservations.Remove(reservation);
            context.SaveChanges();
        }
    }
}
