using LibraryApp1.API.Models;
using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp1.API.Controllers
{
    // Everything here belongs to the signed-in user only.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILibraryService _libraryService;
        private readonly IUserRepository _userRepository;

        public AccountController(IAuthService authService, ILibraryService libraryService, IUserRepository userRepository)
        {
            _authService = authService;
            _libraryService = libraryService;
            _userRepository = userRepository;
        }

        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var user = _userRepository.GetById(User.GetUserId());
            if (user == null)
                return NotFound(new { message = "Account not found." });

            return Ok(new ProfileResponse(user.Id, user.Username, user.Email, user.Role.ToString(), user.CreatedAt));
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            var result = _authService.UpdateProfile(User.GetUserId(), request.Username, request.Email);
            if (!result.IsSuccess)
                return this.Failure(result);

            var user = result.Data!;
            await HttpContext.SignInUserAsync(user);
            return Ok(new UserResponse(user.Id, user.Username, user.Email, user.Role.ToString()));
        }

        [HttpPost("change-password")]
        public IActionResult ChangePassword(ChangePasswordRequest request)
        {
            var result = _authService.ChangePassword(
                User.GetUserId(), request.CurrentPassword, request.NewPassword, request.ConfirmPassword);

            return result.IsSuccess ? Ok(new { message = result.Data }) : this.Failure(result);
        }

        // Current bookings: pending requests, books out, and returns waiting for staff.
        [HttpGet("bookings")]
        public IActionResult GetBookings()
        {
            var all = _libraryService.GetUserReservations(User.GetUserId()).Data ?? new List<ReservationWithBookDto>();
            return Ok(all.Where(r => !IsClosed(r.Status)).ToList());
        }

        // Closed bookings: returned books and rejected requests.
        [HttpGet("history")]
        public IActionResult GetHistory()
        {
            var all = _libraryService.GetUserReservations(User.GetUserId()).Data ?? new List<ReservationWithBookDto>();
            return Ok(all.Where(r => IsClosed(r.Status))
                         .OrderByDescending(r => r.ReturnedAt)
                         .ToList());
        }

        [HttpGet("bookings/{id:int}")]
        public IActionResult GetBooking(int id)
        {
            var result = _libraryService.GetReservationDetails(id, User.GetUserId(), User.GetRole());
            return result.IsSuccess ? Ok(result.Data) : this.Failure(result);
        }

        [HttpPost("bookings/{id:int}/return-request")]
        public IActionResult RequestReturn(int id)
        {
            var result = _libraryService.RequestReturn(id, User.GetUserId());
            return result.IsSuccess ? Ok(new { message = result.Data }) : this.Failure(result);
        }

        private static bool IsClosed(ReservationStatus status) =>
            status == ReservationStatus.Returned || status == ReservationStatus.Rejected;
    }
}
