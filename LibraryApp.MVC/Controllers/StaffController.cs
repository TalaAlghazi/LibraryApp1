using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.MVC.Controllers
{
    // Admin-only: manage staff (librarian) accounts.
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class StaffController : Controller
    {
        private readonly IAuthService _authService;

        public StaffController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Index()
        {
            var result = _authService.GetLibrarians(User.GetRole());
            var librarians = (result.Data ?? new List<AuthenticatedUser>())
                .OrderBy(u => u.Username)
                .ToList();

            return View(librarians);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string username, string email, string password, string confirmPassword)
        {
            var result = _authService.CreateLibrarian(username, email, password, confirmPassword, User.GetRole());

            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
                ? $"{result.Data!.Username} can now sign in as a librarian."
                : result.Description;

            return RedirectToAction(nameof(Index));
        }
    }
}
