using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp1.DataAccess
{
    public class SqlReservationRepository : IReservationRepository
    {
        // SQL Server error numbers for a duplicate key in a unique index or constraint.
        private const int UniqueIndexViolation = 2601;
        private const int UniqueConstraintViolation = 2627;

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
                .OrderBy(r => r.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsEnumerable()
                .Select(ToDto)
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

        public List<ReservationWithBookDto> GetByUser(int userId)
        {
            return context.Reservations
                .Include(r => r.Book)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReservedAt)
                .AsEnumerable()
                .Select(ToDto)
                .ToList();
        }

        public List<ReservationWithBookDto> GetByStatus(ReservationStatus status)
        {
            return context.Reservations
                .Include(r => r.Book)
                .Where(r => r.Status == status)
                .OrderBy(r => r.ReturnRequestedAt)
                .AsEnumerable()
                .Select(ToDto)
                .ToList();
        }

        public ReservationWithBookDto? GetDetails(int reservationId)
        {
            return context.Reservations
                .Include(r => r.Book)
                .Where(r => r.Id == reservationId)
                .AsEnumerable()
                .Select(ToDto)
                .FirstOrDefault();
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

        public bool TryAddWithBook(Reservation reservation, Book book)
        {
            context.Reservations.Add(reservation);
            context.Books.Update(book);

            try
            {
                // One SaveChanges = one transaction: both rows are saved, or neither is.
                context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: UniqueIndexViolation or UniqueConstraintViolation })
            {
                // The unique index allows only one open reservation per book,
                // so another request reserved this book first. Discard the unsaved changes.
                context.ChangeTracker.Clear();
                return false;
            }
        }

        public void UpdateWithBook(Reservation reservation, Book book)
        {
            context.Reservations.Update(reservation);
            context.Books.Update(book);

            // One SaveChanges = one transaction: both rows are saved, or neither is.
            context.SaveChanges();
        }

        private static ReservationWithBookDto ToDto(Reservation r) => new ReservationWithBookDto
        {
            ReservationId = r.Id,
            BookId = r.BookId,
            UserId = r.UserId,
            BookTitle = r.Book?.Title ?? "Unknown",
            BookAuthor = r.Book?.Author ?? "",
            BorrowerName = r.BorrowerName,
            BorrowerPhone = r.BorrowerPhone,
            ReservedAt = r.ReservedAt,
            DueDate = r.DueDate,
            ReturnRequestedAt = r.ReturnRequestedAt,
            ReturnedAt = r.ReturnedAt,
            Fine = r.Fine,
            Status = r.Status
        };
    }
}
