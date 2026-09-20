using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp1.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ILibraryService _libraryService;

        public BooksController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpGet("available")]
        public IActionResult GetAvailableBooks(int pageNumber = 1, int pageSize = 5)
        {
            var result = _libraryService.GetAvailableBooks(pageNumber, pageSize);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Description);
        }

        [HttpGet("reservations")]
        public IActionResult GetActiveReservations(int pageNumber = 1, int pageSize = 5)
        {
            var result = _libraryService.GetActiveReservations(pageNumber, pageSize);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Description);
        }

        [HttpGet("fines")]
        public IActionResult GetFines(int pageNumber = 1, int pageSize = 5)
        {
            var result = _libraryService.GetReservationsWithFines(pageNumber, pageSize);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Description);
        }

        [HttpPost("reserve")]
        public IActionResult ReserveBook([FromQuery] int bookId, [FromQuery] string borrowerName, [FromQuery] string borrowerPhone)

        {
            var result = _libraryService.ReserveBook(bookId, borrowerName, borrowerPhone);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Description);
        }

        [HttpPost("return")]
        public IActionResult ReturnBook([FromQuery] int reservationId)
        {
            var result = _libraryService.ReturnBook(reservationId);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Description);
        }
        [HttpGet("search")]
        public IActionResult SearchBooks([FromQuery] string title, int pageNumber = 1, int pageSize = 5)
        {
            var result = _libraryService.SearchBook(title, pageNumber, pageSize);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Description);
        }

        [HttpPut("reservations/{id}")]
        public IActionResult UpdateReservation(int id, [FromQuery] DateTime newDueDate)
        {
            var result = _libraryService.UpdateReservation(id, newDueDate);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Description);
        }

        [HttpDelete("reservations/{id}")]
        public IActionResult DeleteReservation(int id)
        {
            var result = _libraryService.DeleteReservation(id);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Description);
        }
    }
}