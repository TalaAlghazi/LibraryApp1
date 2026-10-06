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
        private readonly IUserRepository _userRepository;

        public BooksController(ILibraryService libraryService, IUserRepository userRepository)
        {
            _libraryService = libraryService;
            _userRepository = userRepository;
        }

        // GET api/books            -> available books
        // GET api/books?search=x   -> all books whose title contains x
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

        // A customer asks for a book; it stays pending until a librarian hands it over.
        [HttpPost("{bookId:int}/reserve")]
        [Authorize(Roles = nameof(UserRole.Customer))]
        public IActionResult Reserve(int bookId, ReserveRequest request)
        {
            var result = _libraryService.RequestReservation(
                bookId, User.GetUserId(), User.Identity?.Name ?? "", request.BorrowerPhone ?? "");

            return result.IsSuccess ? Ok(new { message = result.Data }) : this.Failure(result);
        }

        // Staff reserve a book for someone at the desk; the loan starts immediately.
        [HttpPost("{bookId:int}/reserve-for")]
        [Authorize(Roles = ApiHelpers.StaffRoles)]
        public IActionResult ReserveForCustomer(int bookId, StaffReserveRequest request)
        {
            int? customerId = null;

            if (!string.IsNullOrWhiteSpace(request.CustomerUsername))
            {
                var customer = _userRepository.GetByUsername(request.CustomerUsername.Trim());
                if (customer == null || customer.Role != UserRole.Customer)
                    return NotFound(new { message = "No customer account with that username." });

                customerId = customer.Id;
            }

            var result = _libraryService.ReserveForCustomer(
                bookId, request.BorrowerName, request.BorrowerPhone, customerId, User.GetRole());

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
