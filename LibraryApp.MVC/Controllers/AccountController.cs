using LibraryApp1.BusinessLogic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LibraryApp.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting(MvcHelpers.AuthRateLimit)]
        public async Task<IActionResult> Login(string username, string password)
        {
            var result = _authService.Login(username, password);
            if (!result.IsSuccess)
            {
                ViewBag.Error = result.Description;
                return View();
            }

            await HttpContext.SignInUserAsync(result.Data!);
            return RedirectToAction("Index", "Books");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Only customers sign up here; librarians are created by the admin on the Staff page.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting(MvcHelpers.AuthRateLimit)]
        public async Task<IActionResult> Register(string username, string email, string password, string confirmPassword)
        {
            var result = _authService.Register(username, email, password, confirmPassword);
            if (!result.IsSuccess)
            {
                ViewBag.Error = result.Description;
                return View();
            }

            await HttpContext.SignInUserAsync(result.Data!);
            return RedirectToAction("Index", "Books");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
