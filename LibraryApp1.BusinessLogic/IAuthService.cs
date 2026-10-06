using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public interface IAuthService
    {
        Result<AuthenticatedUser> Register(string username, string email, string password, string confirmPassword);
        Result<AuthenticatedUser> Login(string username, string password);
        Result<AuthenticatedUser> UpdateProfile(int userId, string username, string email);
        Result<string> ChangePassword(int userId, string currentPassword, string newPassword, string confirmPassword);

        Result<AuthenticatedUser> CreateLibrarian(string username, string email, string password, string confirmPassword, UserRole requesterRole);
        Result<List<AuthenticatedUser>> GetLibrarians(UserRole requesterRole);
        Result<string> EnsureAdminExists(string username, string email, string password);
    }
}