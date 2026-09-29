using System.Security.Claims;
using LibraryApp.MVC.Models;
using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var user = _userRepository.GetById(CurrentUserId);
            if (user == null)
                return RedirectToAction("Logout", "Account");

            return View(ToProfile(user.Username, user.Email, user.Role, user.CreatedAt));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(string username, string email)
        {
            var result = _authService.UpdateProfile(CurrentUserId, username, email);

            if (!result.IsSuccess)
            {
                var user = _userRepository.GetById(CurrentUserId);
                if (user == null)
                    return RedirectToAction("Logout", "Account");

                ViewBag.Error = result.Description;
                return View(ToProfile(username, email, user.Role, user.CreatedAt));
            }

            await RefreshSignInAsync(result.Data!);
            TempData["Success"] = result.Description;
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var result = _authService.ChangePassword(CurrentUserId, currentPassword, newPassword, confirmPassword);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Data : result.Description;
            return RedirectToAction(nameof(Profile));
        }

        public IActionResult Bookings()
        {
            var all = _libraryService.GetUserReservations(CurrentUserId).Data ?? new List<ReservationWithBookDto>();
            return View(all.Where(r => r.Status != ReservationStatus.Returned).ToList());
        }

        public IActionResult History()
        {
            var all = _libraryService.GetUserReservations(CurrentUserId).Data ?? new List<ReservationWithBookDto>();
            return View(all.Where(r => r.Status == ReservationStatus.Returned)
                           .OrderByDescending(r => r.ReturnedAt)
                           .ToList());
        }

        public IActionResult Details(int id)
        {
            var result = _libraryService.GetReservationDetails(id, CurrentUserId, CurrentRole);
            if (!result.IsSuccess)
                return NotFound();

            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestReturn(int id)
        {
            var result = _libraryService.RequestReturn(id, CurrentUserId);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess ? result.Data : result.Description;
            return RedirectToAction(nameof(Bookings));
        }

        private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        private UserRole CurrentRole =>
            Enum.TryParse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value, out var role) ? role : UserRole.Librarian;

        private static ProfileViewModel ToProfile(string username, string email, UserRole role, DateTime createdAt) => new ProfileViewModel
        {
            Username = username,
            Email = email,
            Role = role.ToString(),
            MemberSince = createdAt
        };

        
        private async Task RefreshSignInAsync(AuthenticatedUser user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
    }
}