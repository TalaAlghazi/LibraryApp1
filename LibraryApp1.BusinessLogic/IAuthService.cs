namespace LibraryApp1.BusinessLogic
{
    public interface IAuthService
    {
        Result<AuthenticatedUser> Register(string username, string email, string password, string confirmPassword);
        Result<AuthenticatedUser> Login(string username, string password);
    }
}
