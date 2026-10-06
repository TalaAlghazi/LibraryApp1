using LibraryApp.MVC.Models;
using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LibraryApp.MVC.Controllers
{
    [Authorize]
    public class MyAccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILibraryService _libraryService;
        private readonly IUserRepository _userRepository;

        public MyAccountController(IAuthService authService, ILibraryService libraryService, IUserRepository userRepository)
        {
            _authService = authService;
            _libraryService = libraryService;
            _userRepository = userRepository;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Bookings));
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var user = _userRepository.GetById(User.GetUserId());
            if (user == null)
                return RedirectToAction("Logout", "Account");

            return View(ToProfile(user.Username, user.Email, user.Role, user.CreatedAt));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(string username, string email)
        {
            var result = _authService.UpdateProfile(User.GetUserId(), username, email);

            if (!result.IsSuccess)
            {
                var user = _userRepository.GetById(User.GetUserId());
                if (user == null)
                    return RedirectToAction("Logout", "Account");

                ViewBag.Error = result.Description;
                return View(ToProfile(username, email, user.Role, user.CreatedAt));
            }

            await HttpContext.SignInUserAsync(result.Data!);
            TempData["Success"] = result.Description;
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting(MvcHelpers.AuthRateLimit)]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var result = _authService.ChangePassword(User.GetUserId(), currentPassword, newPassword, confirmPassword);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Data : result.Description;
            return RedirectToAction(nameof(Profile));
        }

        // Current bookings: pending requests, books out, and returns waiting for staff.
        public IActionResult Bookings()
        {
            var all = _libraryService.GetUserReservations(User.GetUserId()).Data ?? new List<ReservationWithBookDto>();
            return View(all.Where(r => !IsClosed(r.Status)).ToList());
        }

        // Closed bookings: returned books and rejected requests.
        public IActionResult History()
        {
            var all = _libraryService.GetUserReservations(User.GetUserId()).Data ?? new List<ReservationWithBookDto>();
            return View(all.Where(r => IsClosed(r.Status))
                           .OrderByDescending(r => r.ReturnedAt)
                           .ToList());
        }

        public IActionResult Details(int id)
        {
            var result = _libraryService.GetReservationDetails(id, User.GetUserId(), User.GetRole());
            if (!result.IsSuccess)
                return NotFound();

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestReturn(int id)
        {
            var result = _libraryService.RequestReturn(id, User.GetUserId());
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Data : result.Description;
            return RedirectToAction(nameof(Bookings));
        }

        private static bool IsClosed(ReservationStatus status) =>
            status == ReservationStatus.Returned || status == ReservationStatus.Rejected;

        private static ProfileViewModel ToProfile(string username, string email, UserRole role, DateTime createdAt) => new ProfileViewModel
        {
            Username = username,
            Email = email,
            Role = role.ToString(),
            MemberSince = createdAt
        };
    }
}
