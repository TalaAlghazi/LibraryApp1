using LibraryApp1.API.Models;
using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp1.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly ILibraryService _libraryService;

        public BooksController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpGet]
        public IActionResult GetBooks(string? search, int pageNumber = 1, int pageSize = 20)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var result = string.IsNullOrWhiteSpace(search)
                ? _libraryService.GetAvailableBooks(pageNumber, pageSize)
                : _libraryService.SearchBook(search, pageNumber, pageSize);

            return result.IsSuccess ? Ok(result.Data) : this.Failure(result);
        }

        [HttpPost("{bookId:int}/reserve")]
        public IActionResult Reserve(int bookId, ReserveRequest request)
        {
            var result = _libraryService.ReserveBook(
                bookId, User.Identity?.Name ?? "", request.BorrowerPhone ?? "", User.GetUserId());

            return result.IsSuccess ? Ok(new { message = result.Data }) : this.Failure(result);
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public IActionResult AddBook(AddBookRequest request)
        {
            var result = _libraryService.AddBook(request.Title, request.Author, User.GetRole());
            return result.IsSuccess ? Ok(result.Data) : this.Failure(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public IActionResult DeleteBook(int id)
        {
            var result = _libraryService.DeleteBook(id, User.GetRole());
            return result.IsSuccess ? Ok(new { message = result.Data }) : this.Failure(result);
        }
    }
}