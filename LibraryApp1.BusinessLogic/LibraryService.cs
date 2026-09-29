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

        public Result<string> ReserveBook(int bookId, string borrowerName, string borrowerPhone, int? userId = null)
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
                UserId = userId,
                BorrowerName = borrowerName,
                BorrowerPhone = borrowerPhone,
                ReservedAt = DateTime.Now,
                DueDate = DateTime.Now.AddDays(book.GetLoanPeriodDays()),
                Status = ReservationStatus.Active
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

            return CompleteReturn(reservation, book);
        }

        public Result<Book> AddBook(string title, string author, UserRole requesterRole)
        {
            if (requesterRole != UserRole.Admin)
                return Result<Book>.Failure(403, "Only administrators can add books.");

            if (string.IsNullOrWhiteSpace(title))
                return Result<Book>.Failure(400, "Title is required.");

            if (string.IsNullOrWhiteSpace(author))
                return Result<Book>.Failure(400, "Author is required.");

            var book = new Book
            {
                Title = title.Trim(),
                Author = author.Trim(),
                IsAvailable = true
            };

            bookRepository.Add(book);

            return Result<Book>.Success(book, "Book added successfully.");
        }

        public Result<string> DeleteBook(int bookId, UserRole requesterRole)
        {
            if (requesterRole != UserRole.Admin)
                return Result<string>.Failure(403, "Only administrators can delete books.");

            var book = bookRepository.GetById(bookId);
            if (book == null)
                return Result<string>.Failure(404, "Book not found.");

            var activeReservation = reservationRepository.GetActiveByBookId(bookId);
            if (activeReservation != null)
                return Result<string>.Failure(409, "Cannot delete a book that is currently reserved.");

            bookRepository.Delete(bookId);

            return Result<string>.Success("Book deleted successfully.");
        }

        public Result<List<ReservationWithBookDto>> GetUserReservations(int userId)
        {
            return Result<List<ReservationWithBookDto>>.Success(reservationRepository.GetByUser(userId));
        }

        public Result<ReservationWithBookDto> GetReservationDetails(int reservationId, int userId, UserRole role)
        {
            var reservation = reservationRepository.GetDetails(reservationId);

            
            if (reservation == null || (role != UserRole.Admin && reservation.UserId != userId))
                return Result<ReservationWithBookDto>.Failure(404, "Booking not found.");

            return Result<ReservationWithBookDto>.Success(reservation);
        }

        public Result<string> RequestReturn(int reservationId, int userId)
        {
            var reservation = reservationRepository.GetById(reservationId);

            if (reservation == null || reservation.UserId != userId)
                return Result<string>.Failure(404, "Booking not found.");

            if (reservation.Status == ReservationStatus.ReturnRequested)
                return Result<string>.Failure(409, "A return has already been requested for this booking.");

            if (reservation.Status == ReservationStatus.Returned)
                return Result<string>.Failure(409, "This booking has already been returned.");

            reservation.Status = ReservationStatus.ReturnRequested;
            reservation.ReturnRequestedAt = DateTime.Now;
            reservationRepository.Update(reservation);

            return Result<string>.Success("Return requested. A staff member will confirm it once the book is received.");
        }

        public Result<List<ReservationWithBookDto>> GetPendingReturns(UserRole role)
        {
            if (role != UserRole.Admin)
                return Result<List<ReservationWithBookDto>>.Failure(403, "Only administrators can view pending returns.");

            return Result<List<ReservationWithBookDto>>.Success(
                reservationRepository.GetByStatus(ReservationStatus.ReturnRequested));
        }

        public Result<string> ConfirmReturn(int reservationId, UserRole role)
        {
            if (role != UserRole.Admin)
                return Result<string>.Failure(403, "Only administrators can confirm returns.");

            var reservation = reservationRepository.GetById(reservationId);
            if (reservation == null)
                return Result<string>.Failure(404, "Booking not found.");

            if (reservation.Status == ReservationStatus.Returned)
                return Result<string>.Failure(409, "This booking has already been returned.");

            var book = bookRepository.GetById(reservation.BookId);
            if (book == null)
                return Result<string>.Failure(404, "Book not found.");

            return CompleteReturn(reservation, book);
        }

        private Result<string> CompleteReturn(Reservation reservation, Book book)
        {
            
            var returnDate = reservation.ReturnRequestedAt ?? DateTime.Now;

            reservation.ReturnedAt = DateTime.Now;
            reservation.Fine = CalculateFine(reservation.DueDate, book.GetFinePerDay(), returnDate);
            reservation.Status = ReservationStatus.Returned;
            reservationRepository.Update(reservation);

            book.IsAvailable = true;
            bookRepository.Update(book);

            string message = reservation.Fine > 0
                ? $"Book returned late. Fine: ${reservation.Fine}"
                : "Book returned on time. No fine.";

            return Result<string>.Success(message);
        }

        private static decimal CalculateFine(DateTime dueDate, decimal finePerDay, DateTime returnDate)
        {
            int daysLate = (int)(returnDate - dueDate).TotalDays;
            return daysLate > 0 ? daysLate * finePerDay : 0;
        }
    }
}