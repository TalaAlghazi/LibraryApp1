namespace LibraryApp1.API.Models
{
    public record ReserveRequest(string? BorrowerPhone);

    public record StaffReserveRequest(string BorrowerName, string BorrowerPhone, string? CustomerUsername);

    public record AddBookRequest(string Title, string Author);

    public record UpdateProfileRequest(string Username, string Email);

    public record ChangePasswordRequest(string CurrentPassword, string NewPassword, string ConfirmPassword);

    public record ProfileResponse(int Id, string Username, string Email, string Role, DateTime MemberSince);

    public record CreateLibrarianRequest(string Username, string Email, string Password, string ConfirmPassword);
}
