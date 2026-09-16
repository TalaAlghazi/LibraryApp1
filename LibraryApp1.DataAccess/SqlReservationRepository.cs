using Microsoft.EntityFrameworkCore;

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

        public List<ReservationWithBookDto> GetAllWithBooks(
            int pageNumber,
            int pageSize,
            bool? isActive = null,
            bool? hasFine = null,
            string? borrowerName = null)
        {
            IQueryable<Reservation> query = context.Reservations.Include(r => r.Book);

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
                .AsEnumerable()
                .Select(r => new ReservationWithBookDto
                {
                    ReservationId = r.Id,
                    BookId = r.BookId,
                    BookTitle = r.Book?.Title ?? "Unknown",
                    BorrowerName = r.BorrowerName,
                    BorrowerPhone = r.BorrowerPhone,
                    ReservedAt = r.ReservedAt,
                    DueDate = r.DueDate,
                    ReturnedAt = r.ReturnedAt,
                    Fine = r.Fine
                })
                .ToList();
        }

        public List<ReservationWithBookDto> GetActiveWithBooks(int pageNumber, int pageSize)
        {
            return GetAllWithBooks(pageNumber, pageSize, isActive: true);
        }

        public List<ReservationWithBookDto> GetWithFinesAndBooks(int pageNumber, int pageSize)
        {
            return GetAllWithBooks(pageNumber, pageSize, hasFine: true);
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