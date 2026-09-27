using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public class AuthenticatedUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public UserRole Role { get; set; }
    }
}