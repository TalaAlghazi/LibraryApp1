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
            // Search follows the same rule as the book list: reserved books are not offered.
            var books = bookRepository.GetAll(pageNumber, pageSize, isAvailable: true, titleContains: title);
            return Result<List<Book>>.Success(books);
        }

        public Result<string> UpdateReservation(int reservationId, DateTime newDueDate)
        {
            var reservation = reservationRepository.GetById(reservationId);
            if (reservation == null)
                return Result<string>.Failure(404, "Reservation not found.");

            // Only books that are out with a customer have a real due date.
            if (reservation.Status != ReservationStatus.Active && reservation.Status != ReservationStatus.ReturnRequested)
                return Result<string>.Failure(409, "Only active bookings can have their due date changed.");

            if (newDueDate.Date < reservation.ReservedAt.Date)
                return Result<string>.Failure(400, "Due date cannot be before the reservation date.");

            reservation.DueDate = newDueDate;
            reservationRepository.Update(reservation);
            return Result<string>.Success($"Due date updated to {newDueDate:d}.");
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

        // Direct reservation: the book is handed over now, so the loan starts immediately.
        public Result<string> ReserveBook(int bookId, string borrowerName, string borrowerPhone, int? userId = null)
        {
            return CreateReservation(bookId, borrowerName, borrowerPhone, userId, ReservationStatus.Active);
        }

        // A customer's online request: the book is held until staff hand it over.
        public Result<string> RequestReservation(int bookId, int userId, string borrowerName, string borrowerPhone)
        {
            return CreateReservation(bookId, borrowerName, borrowerPhone, userId, ReservationStatus.Pending);
        }

        // Staff reserve a book for someone at the desk.
        public Result<string> ReserveForCustomer(int bookId, string borrowerName, string borrowerPhone, int? customerUserId, UserRole requesterRole)
        {
            if (!IsStaff(requesterRole))
                return Result<string>.Failure(403, "Only library staff can reserve books for customers.");

            if (string.IsNullOrWhiteSpace(borrowerPhone))
                return Result<string>.Failure(400, "Phone number is required.");

            return CreateReservation(bookId, borrowerName, borrowerPhone, customerUserId, ReservationStatus.Active);
        }

        public Result<List<ReservationWithBookDto>> GetPendingRequests(UserRole role)
        {
            if (!IsStaff(role))
                return Result<List<ReservationWithBookDto>>.Failure(403, "Only library staff can view pending requests.");

            return Result<List<ReservationWithBookDto>>.Success(
                reservationRepository.GetByStatus(ReservationStatus.Pending));
        }

        public Result<string> HandOverReservation(int reservationId, UserRole role)
        {
            if (!IsStaff(role))
                return Result<string>.Failure(403, "Only library staff can hand over books.");

            var reservation = reservationRepository.GetById(reservationId);
            if (reservation == null)
                return Result<string>.Failure(404, "Request not found.");

            if (reservation.Status != ReservationStatus.Pending)
                return Result<string>.Failure(409, "This request is no longer waiting for pickup.");

            var book = bookRepository.GetById(reservation.BookId);
            if (book == null)
                return Result<string>.Failure(404, "Book not found.");

            // The loan period starts when the customer collects the book.
            reservation.ReservedAt = DateTime.Now;
            reservation.DueDate = DateTime.Now.AddDays(book.GetLoanPeriodDays());
            reservation.Status = ReservationStatus.Active;
            reservationRepository.Update(reservation);

            return Result<string>.Success($"Book handed over. Due date: {reservation.DueDate:d}");
        }

        public Result<string> RejectReservation(int reservationId, UserRole role)
        {
            if (!IsStaff(role))
                return Result<string>.Failure(403, "Only library staff can reject requests.");

            var reservation = reservationRepository.GetById(reservationId);
            if (reservation == null)
                return Result<string>.Failure(404, "Request not found.");

            if (reservation.Status != ReservationStatus.Pending)
                return Result<string>.Failure(409, "This request is no longer waiting for pickup.");

            var book = bookRepository.GetById(reservation.BookId);
            if (book == null)
                return Result<string>.Failure(404, "Book not found.");

            // Closing the request frees the book for other customers.
            reservation.Status = ReservationStatus.Rejected;
            reservation.ReturnedAt = DateTime.Now;
            book.IsAvailable = true;
            reservationRepository.UpdateWithBook(reservation, book);

            return Result<string>.Success("Request rejected. The book is available again.");
        }

        // Direct return by staff (used by the desktop app, console and API).
        public Result<string> ReturnBook(int bookId)
        {
            var book = bookRepository.GetById(bookId);

            if (book == null)
                return Result<string>.Failure(404, "Book not found.");

            var reservation = reservationRepository.GetActiveByBookId(bookId);

            if (reservation == null)
                return Result<string>.Failure(409, "Book is already available.");

            if (reservation.Status == ReservationStatus.Pending)
                return Result<string>.Failure(409, "This book is waiting to be handed over, not returned.");

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

            // Same message whether it doesn't exist or belongs to someone else.
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

            if (reservation.Status == ReservationStatus.Pending)
                return Result<string>.Failure(409, "This book has not been handed over yet.");

            if (reservation.Status != ReservationStatus.Active)
                return Result<string>.Failure(409, "This booking is already closed.");

            reservation.Status = ReservationStatus.ReturnRequested;
            reservation.ReturnRequestedAt = DateTime.Now;
            reservationRepository.Update(reservation);

            return Result<string>.Success("Return requested. A staff member will confirm it once the book is received.");
        }

        public Result<List<ReservationWithBookDto>> GetPendingReturns(UserRole role)
        {
            if (!IsStaff(role))
                return Result<List<ReservationWithBookDto>>.Failure(403, "Only library staff can view pending returns.");

            return Result<List<ReservationWithBookDto>>.Success(
                reservationRepository.GetByStatus(ReservationStatus.ReturnRequested));
        }

        public Result<string> ConfirmReturn(int reservationId, UserRole role)
        {
            if (!IsStaff(role))
                return Result<string>.Failure(403, "Only library staff can confirm returns.");

            var reservation = reservationRepository.GetById(reservationId);
            if (reservation == null)
                return Result<string>.Failure(404, "Booking not found.");

            if (reservation.Status != ReservationStatus.Active && reservation.Status != ReservationStatus.ReturnRequested)
                return Result<string>.Failure(409, "Only books that are out can be marked as returned.");

            var book = bookRepository.GetById(reservation.BookId);
            if (book == null)
                return Result<string>.Failure(404, "Book not found.");

            return CompleteReturn(reservation, book);
        }

        private Result<string> CreateReservation(int bookId, string borrowerName, string borrowerPhone, int? userId, ReservationStatus status)
        {
            if (string.IsNullOrWhiteSpace(borrowerName))
                return Result<string>.Failure(400, "Borrower name cannot be empty.");

            var book = bookRepository.GetById(bookId);

            if (book == null)
                return Result<string>.Failure(404, "Book not found.");

            if (!book.IsAvailable)
                return Result<string>.Failure(409, "Book is already reserved.");

            bool isRequest = status == ReservationStatus.Pending;
            var now = DateTime.Now;

            var reservation = new Reservation
            {
                BookId = book.Id,
                UserId = userId,
                BorrowerName = borrowerName.Trim(),
                BorrowerPhone = borrowerPhone?.Trim(),
                ReservedAt = now,
                // A request has no loan period yet; the due date is set when the book is handed over.
                DueDate = isRequest ? now : now.AddDays(book.GetLoanPeriodDays()),
                Status = status
            };

            // Saved together with the book. If someone else reserved it a moment ago,
            // the database rejects this one instead of creating a second reservation.
            book.IsAvailable = false;
            if (!reservationRepository.TryAddWithBook(reservation, book))
                return Result<string>.Failure(409, "Book is already reserved.");

            return Result<string>.Success(isRequest
                ? "Request sent. Collect the book at the library; your loan starts when it is handed over."
                : $"Book reserved successfully! Due date: {reservation.DueDate:d}");
        }

        private Result<string> CompleteReturn(Reservation reservation, Book book)
        {
            // Fine is based on when the user handed it in, not when staff confirmed it.
            var returnDate = reservation.ReturnRequestedAt ?? DateTime.Now;

            reservation.ReturnedAt = DateTime.Now;
            reservation.Fine = CalculateFine(reservation.DueDate, book.GetFinePerDay(), returnDate);
            reservation.Status = ReservationStatus.Returned;
            book.IsAvailable = true;
            reservationRepository.UpdateWithBook(reservation, book);

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

        private static bool IsStaff(UserRole role) => role == UserRole.Admin || role == UserRole.Librarian;
    }
}
