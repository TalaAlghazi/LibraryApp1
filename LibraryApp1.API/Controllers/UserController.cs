using LibraryApp1.API.Models;
using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp1.API.Controllers
{
    // Admin-only: manage staff (librarian) accounts.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UsersController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("librarians")]
        public IActionResult GetLibrarians()
        {
            var result = _authService.GetLibrarians(User.GetRole());
            return result.IsSuccess ? Ok(result.Data!.Select(ToResponse)) : this.Failure(result);
        }

        [HttpPost("librarians")]
        public IActionResult CreateLibrarian(CreateLibrarianRequest request)
        {
            var result = _authService.CreateLibrarian(
                request.Username, request.Email, request.Password, request.ConfirmPassword, User.GetRole());

            return result.IsSuccess ? Ok(ToResponse(result.Data!)) : this.Failure(result);
        }

        private static UserResponse ToResponse(AuthenticatedUser user) =>
            new(user.Id, user.Username, user.Email, user.Role.ToString());
    }
}
