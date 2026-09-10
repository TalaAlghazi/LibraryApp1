using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public class LibraryService : ILibraryService
    {
        private readonly IBookRepository bookRepository;
        private readonly IReservationRepository reservationRepository;

        public LibraryService(
            IBookRepository bookRepository,
            IReservationRepository reservationRepository)
        {
            this.bookRepository = bookRepository;
            this.reservationRepository = reservationRepository;
        }

        public Result<List<Book>> GetAvailableBooks(int pageNumber, int pageSize)
        {
            var books = bookRepository.GetAll(pageNumber, pageSize, isAvailable: true);
            return Result<List<Book>>.Success(books);
        }

        public Result<List<Book>> SearchBook(string title, int pageNumber, int pageSize)
        {
            var books = bookRepository.GetAll(pageNumber, pageSize, titleContains: title);
            return Result<List<Book>>.Success(books);
        }

        public Result<List<Reservation>> GetActiveReservations(int pageNumber, int pageSize)
        {
            var reservations = reservationRepository.GetAll(pageNumber, pageSize, isActive: true);
            return Result<List<Reservation>>.Success(reservations);
        }

        public Result<List<Reservation>> GetReservationsWithFines(int pageNumber, int pageSize)
        {
            var reservations = reservationRepository.GetAll(pageNumber, pageSize, hasFine: true);
            return Result<List<Reservation>>.Success(reservations);
        }

        public Result<Reservation> ReserveBook(int bookId, string borrowerName)
        {
            if (string.IsNullOrWhiteSpace(borrowerName))
                return Result<Reservation>.Failure(400, "Borrower name cannot be empty.");

            var book = bookRepository.GetById(bookId);

            if (book == null)
                return Result<Reservation>.Failure(404, "Book not found.");

            if (!book.IsAvailable)
                return Result<Reservation>.Failure(409, "Book is already reserved.");

            var reservation = new Reservation
            {
                BookId = book.Id,
                BorrowerName = borrowerName,
                ReservedAt = DateTime.Now,
                DueDate = DateTime.Now.AddDays(book.GetLoanPeriodDays())
            };

            reservationRepository.Add(reservation);

            book.IsAvailable = false;
            bookRepository.Update(book);

            return Result<Reservation>.Success(
                reservation,
                $"Book reserved successfully! Due date: {reservation.DueDate:d}");
        }

        public Result<Reservation> ReturnBook(int bookId)
        {
            var book = bookRepository.GetById(bookId);

            if (book == null)
                return Result<Reservation>.Failure(404, "Book not found.");

            var reservation = reservationRepository.GetActiveByBookId(bookId);

            if (reservation == null)
                return Result<Reservation>.Failure(409, "Book is already available.");

            reservation.ReturnedAt = DateTime.Now;
            reservation.Fine = CalculateFine(reservation.DueDate, book.GetFinePerDay());
            reservationRepository.Update(reservation);

            book.IsAvailable = true;
            bookRepository.Update(book);

            string message = reservation.Fine > 0
                ? $"Book returned late. Fine: ${reservation.Fine}"
                : "Book returned on time. No fine.";

            return Result<Reservation>.Success(reservation, message);
        }

        private static decimal CalculateFine(DateTime dueDate, decimal finePerDay)
        {
            int daysLate = (int)(DateTime.Now - dueDate).TotalDays;
            return daysLate > 0 ? daysLate * finePerDay : 0;
        }
    }
}