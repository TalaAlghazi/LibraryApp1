namespace LibraryApp1.API.Models
{
    public record RegisterRequest(string Username, string Email, string Password, string ConfirmPassword);

    public record LoginRequest(string Username, string Password);

    public record UserResponse(int Id, string Username, string Email, string Role);
}