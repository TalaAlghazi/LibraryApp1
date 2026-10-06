using System.Security.Claims;
using LibraryApp1.API.Models;
using LibraryApp1.BusinessLogic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LibraryApp1.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [EnableRateLimiting(ApiHelpers.AuthRateLimit)]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = _authService.Register(request.Username, request.Email, request.Password, request.ConfirmPassword);
            if (!result.IsSuccess)
                return StatusCode(result.Code, new { message = result.Description });

            await SignInAsync(result.Data!);
            return Ok(ToResponse(result.Data!));
        }

        [HttpPost("login")]
        [EnableRateLimiting(ApiHelpers.AuthRateLimit)]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = _authService.Login(request.Username, request.Password);
            if (!result.IsSuccess)
                return StatusCode(result.Code, new { message = result.Description });

            await SignInAsync(result.Data!);
            return Ok(ToResponse(result.Data!));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logged out successfully." });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok(new
            {
                Username = User.Identity?.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }

        private async Task SignInAsync(AuthenticatedUser user)
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

        private static UserResponse ToResponse(AuthenticatedUser user) =>
            new(user.Id, user.Username, user.Email, user.Role.ToString());
    }
}