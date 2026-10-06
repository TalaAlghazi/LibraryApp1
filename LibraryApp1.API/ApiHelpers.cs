using System.Security.Claims;
using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp1.API
{
    public static class ApiHelpers
    {
        // Roles that run the library desk (used in [Authorize(Roles = ...)]).
        public const string StaffRoles = nameof(UserRole.Admin) + "," + nameof(UserRole.Librarian);

        public static int GetUserId(this ClaimsPrincipal user) =>
            int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        public static UserRole GetRole(this ClaimsPrincipal user) =>
            Enum.TryParse<UserRole>(user.FindFirst(ClaimTypes.Role)?.Value, out var role) ? role : UserRole.Customer;

        // Turns a failed service result into the matching HTTP status with a JSON message.
        public static IActionResult Failure<T>(this ControllerBase controller, Result<T> result) =>
            controller.StatusCode(result.Code >= 400 ? result.Code : 400, new { message = result.Description });

        // Username and email live in the auth cookie, so it is re-issued after a profile change.
        public static Task SignInUserAsync(this HttpContext httpContext, AuthenticatedUser user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }
    }
}
