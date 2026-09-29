using System.Security.Claims;
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

        public BooksController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        public IActionResult Index(string? search)
        {
            var result = string.IsNullOrWhiteSpace(search)
                ? _libraryService.GetAvailableBooks(1, 50)
                : _libraryService.SearchBook(search, 1, 50);

            ViewBag.Search = search;
            return View(result.Data ?? new List<Book>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reserve(int bookId, string? borrowerPhone)
        {
            var result = _libraryService.ReserveBook(bookId, User.Identity?.Name ?? "", borrowerPhone ?? "", CurrentUserId);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Description;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = result.Data;
            return RedirectToAction("Bookings", "MyAccount");
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        public IActionResult Reservations()
        {
            var result = _libraryService.GetActiveReservations(1, 200);
            var list = (result.Data ?? new List<ReservationWithBookDto>())
                .OrderByDescending(r => r.Status == ReservationStatus.ReturnRequested)
                .ThenBy(r => r.DueDate)
                .ToList();

            return View(list);
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmReturn(int reservationId)
        {
            var result = _libraryService.ConfirmReturn(reservationId, CurrentRole);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Data : result.Description;
            return RedirectToAction(nameof(Reservations));
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(string title, string author)
        {
            var result = _libraryService.AddBook(title, author, CurrentRole);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? "Book added successfully." : result.Description;
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int bookId)
        {
            var result = _libraryService.DeleteBook(bookId, CurrentRole);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Data : result.Description;
            return RedirectToAction(nameof(Index));
        }

        private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        private UserRole CurrentRole =>
            Enum.TryParse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value, out var role) ? role : UserRole.Librarian;
    }
}
