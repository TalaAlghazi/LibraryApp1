using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.MVC.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ILibraryService _libraryService;
        private readonly IUserRepository _userRepository;

        public BooksController(ILibraryService libraryService, IUserRepository userRepository)
        {
            _libraryService = libraryService;
            _userRepository = userRepository;
        }

        public IActionResult Index(string? search)
        {
            var result = string.IsNullOrWhiteSpace(search)
                ? _libraryService.GetAvailableBooks(1, 50)
                : _libraryService.SearchBook(search, 1, 50);

            ViewBag.Search = search;
            return View(result.Data ?? new List<Book>());
        }

        // A customer asks for a book; it stays pending until a librarian hands it over.
        [Authorize(Roles = nameof(UserRole.Customer))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reserve(int bookId, string? borrowerPhone)
        {
            var result = _libraryService.RequestReservation(
                bookId, User.GetUserId(), User.Identity?.Name ?? "", borrowerPhone ?? "");

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Description;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = result.Data;
            return RedirectToAction("Bookings", "MyAccount");
        }

        // Staff reserve a book for someone at the desk; the loan starts immediately.
        [Authorize(Roles = MvcHelpers.StaffRoles)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReserveFor(int bookId, string? borrowerName, string? borrowerPhone, string? customerUsername)
        {
            int? customerId = null;

            if (!string.IsNullOrWhiteSpace(customerUsername))
            {
                var customer = _userRepository.GetByUsername(customerUsername.Trim());
                if (customer == null || customer.Role != UserRole.Customer)
                {
                    TempData["Error"] = "No customer account with that username.";
                    return RedirectToAction(nameof(Index));
                }

                customerId = customer.Id;
            }

            var result = _libraryService.ReserveForCustomer(
                bookId, borrowerName ?? "", borrowerPhone ?? "", customerId, User.GetRole());

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Description;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = result.Data;
            return RedirectToAction(nameof(Reservations));
        }

        // Books that are out with customers. Pending requests have their own page.
        [Authorize(Roles = MvcHelpers.StaffRoles)]
        public IActionResult Reservations()
        {
            var result = _libraryService.GetActiveReservations(1, 200);
            var list = (result.Data ?? new List<ReservationWithBookDto>())
                .Where(r => r.Status != ReservationStatus.Pending)
                .OrderByDescending(r => r.Status == ReservationStatus.ReturnRequested)
                .ThenBy(r => r.DueDate)
                .ToList();

            return View(list);
        }

        [Authorize(Roles = MvcHelpers.StaffRoles)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmReturn(int reservationId)
        {
            var result = _libraryService.ConfirmReturn(reservationId, User.GetRole());
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Data : result.Description;
            return RedirectToAction(nameof(Reservations));
        }

        // Customer requests waiting for the book to be collected.
        [Authorize(Roles = MvcHelpers.StaffRoles)]
        public IActionResult Requests()
        {
            var result = _libraryService.GetPendingRequests(User.GetRole());
            var list = (result.Data ?? new List<ReservationWithBookDto>())
                .OrderBy(r => r.ReservedAt)
                .ToList();

            return View(list);
        }

        [Authorize(Roles = MvcHelpers.StaffRoles)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HandOver(int reservationId)
        {
            var result = _libraryService.HandOverReservation(reservationId, User.GetRole());
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Data : result.Description;
            return RedirectToAction(nameof(Requests));
        }

        [Authorize(Roles = MvcHelpers.StaffRoles)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int reservationId)
        {
            var result = _libraryService.RejectReservation(reservationId, User.GetRole());
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Data : result.Description;
            return RedirectToAction(nameof(Requests));
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(string title, string author)
        {
            var result = _libraryService.AddBook(title, author, User.GetRole());
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? "Book added successfully." : result.Description;
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int bookId)
        {
            var result = _libraryService.DeleteBook(bookId, User.GetRole());
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Data : result.Description;
            return RedirectToAction(nameof(Index));
        }
    }
}
