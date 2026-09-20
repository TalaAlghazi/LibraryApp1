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
        public Result<string> UpdateReservation(int reservationId, DateTime newDueDate)
        {
            var reservation = reservationRepository.GetById(reservationId);
            if (reservation == null)
                return new Result<string> { IsSuccess = false, Description = "Not found" };

            reservation.DueDate = newDueDate;
            reservationRepository.Update(reservation);
            return new Result<string> { IsSuccess = true, Data = "Updated successfully" };
        }

        public Result<string> DeleteReservation(int reservationId)
        {
            var reservation = reservationRepository.GetById(reservationId);
            if (reservation == null)
                return new Result<string> { IsSuccess = false, Description = "Not found" };

            reservationRepository.Delete(reservationId);
            return new Result<string> { IsSuccess = true, Data = "Deleted successfully" };
        }

        public Result<List<ReservationWithBookDto>> GetActiveReservations(int pageNumber, int pageSize)
        {
            var reservations = reservationRepository.GetActiveWithBooks(pageNumber, pageSize);
            return Result<List<ReservationWithBookDto>>.Success(reservations);
        }

        public Result<List<ReservationWithBookDto>> GetReservationsWithFines(int pageNumber, int pageSize)
        {
            var reservations = reservationRepository.GetWithFinesAndBooks(pageNumber, pageSize);
            return Result<List<ReservationWithBookDto>>.Success(reservations);
        }

        public Result<string> ReserveBook(int bookId, string borrowerName, string borrowerPhone)
        {
            if (string.IsNullOrWhiteSpace(borrowerName))
                return Result<string>.Failure(400, "Borrower name cannot be empty.");

            var book = bookRepository.GetById(bookId);

            if (book == null)
                return Result<string>.Failure(404, "Book not found.");

            if (!book.IsAvailable)
                return Result<string>.Failure(409, "Book is already reserved.");

            var reservation = new Reservation
            {
                BookId = book.Id,
                BorrowerName = borrowerName,
                BorrowerPhone = borrowerPhone,
                ReservedAt = DateTime.Now,
                DueDate = DateTime.Now.AddDays(book.GetLoanPeriodDays())
            };

            reservationRepository.Add(reservation);

            book.IsAvailable = false;
            bookRepository.Update(book);

            return Result<string>.Success(
                $"Book reserved successfully! Due date: {reservation.DueDate:d}");
        }

        public Result<string> ReturnBook(int bookId)
        {
            var book = bookRepository.GetById(bookId);

            if (book == null)
                return Result<string>.Failure(404, "Book not found.");

            var reservation = reservationRepository.GetActiveByBookId(bookId);

            if (reservation == null)
                return Result<string>.Failure(409, "Book is already available.");

            reservation.ReturnedAt = DateTime.Now;
            reservation.Fine = CalculateFine(reservation.DueDate, book.GetFinePerDay());
            reservationRepository.Update(reservation);

            book.IsAvailable = true;
            bookRepository.Update(book);

            string message = reservation.Fine > 0
                ? $"Book returned late. Fine: ${reservation.Fine}"
                : "Book returned on time. No fine.";

            return Result<string>.Success( message);
        }

        private static decimal CalculateFine(DateTime dueDate, decimal finePerDay)
        {
            int daysLate = (int)(DateTime.Now - dueDate).TotalDays;
            return daysLate > 0 ? daysLate * finePerDay : 0;
        }
    }
}