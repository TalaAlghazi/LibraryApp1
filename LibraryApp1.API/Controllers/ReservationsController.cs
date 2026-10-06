using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp1.API.Controllers
{
    // Library desk (librarians and admins): requests, books that are out, returns.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = ApiHelpers.StaffRoles)]
    public class ReservationsController : ControllerBase
    {
        private readonly ILibraryService _libraryService;

        public ReservationsController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        // All bookings that are not closed yet (pending requests included).
        [HttpGet]
        public IActionResult GetActive(int pageNumber = 1, int pageSize = 50)
        {
            var result = _libraryService.GetActiveReservations(Math.Max(1, pageNumber), Math.Clamp(pageSize, 1, 200));
            return result.IsSuccess ? Ok(result.Data) : this.Failure(result);
        }

        // Customer requests waiting to be handed over.
        [HttpGet("pending-requests")]
        public IActionResult GetPendingRequests()
        {
            var result = _libraryService.GetPendingRequests(User.GetRole());
            return result.IsSuccess ? Ok(result.Data) : this.Failure(result);
        }

        [HttpPost("{id:int}/hand-over")]
        public IActionResult HandOver(int id)
        {
            var result = _libraryService.HandOverReservation(id, User.GetRole());
            return result.IsSuccess ? Ok(new { message = result.Data }) : this.Failure(result);
        }

        [HttpPost("{id:int}/reject")]
        public IActionResult Reject(int id)
        {
            var result = _libraryService.RejectReservation(id, User.GetRole());
            return result.IsSuccess ? Ok(new { message = result.Data }) : this.Failure(result);
        }

        [HttpGet("pending-returns")]
        public IActionResult GetPendingReturns()
        {
            var result = _libraryService.GetPendingReturns(User.GetRole());
            return result.IsSuccess ? Ok(result.Data) : this.Failure(result);
        }

        [HttpGet("fines")]
        public IActionResult GetFines(int pageNumber = 1, int pageSize = 50)
        {
            var result = _libraryService.GetReservationsWithFines(Math.Max(1, pageNumber), Math.Clamp(pageSize, 1, 200));
            return result.IsSuccess ? Ok(result.Data) : this.Failure(result);
        }

        [HttpPost("{id:int}/confirm-return")]
        public IActionResult ConfirmReturn(int id)
        {
            var result = _libraryService.ConfirmReturn(id, User.GetRole());
            return result.IsSuccess ? Ok(new { message = result.Data }) : this.Failure(result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public IActionResult UpdateDueDate(int id, [FromQuery] DateTime newDueDate)
        {
            var result = _libraryService.UpdateReservation(id, newDueDate);
            return result.IsSuccess ? Ok(new { message = result.Data }) : this.Failure(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public IActionResult Delete(int id)
        {
            var result = _libraryService.DeleteReservation(id);
            return result.IsSuccess ? Ok(new { message = result.Data }) : this.Failure(result);
        }
    }
}
