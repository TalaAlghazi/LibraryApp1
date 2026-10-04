using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp1.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public class ReservationsController : ControllerBase
    {
        private readonly ILibraryService _libraryService;

        public ReservationsController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        
        [HttpGet]
        public IActionResult GetActive(int pageNumber = 1, int pageSize = 50)
        {
            var result = _libraryService.GetActiveReservations(Math.Max(1, pageNumber), Math.Clamp(pageSize, 1, 200));
            return result.IsSuccess ? Ok(result.Data) : this.Failure(result);
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
        public IActionResult UpdateDueDate(int id, [FromQuery] DateTime newDueDate)
        {
            var result = _libraryService.UpdateReservation(id, newDueDate);
            return result.IsSuccess ? Ok(new { message = result.Data }) : this.Failure(result);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var result = _libraryService.DeleteReservation(id);
            return result.IsSuccess ? Ok(new { message = result.Data }) : this.Failure(result);
        }
    }
}