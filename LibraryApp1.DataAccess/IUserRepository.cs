namespace LibraryApp1.DataAccess
{
    public interface IUserRepository
    {
        User? GetById(int id);
        User? GetByUsername(string username);
        User? GetByEmail(string email);
        int Count();
        bool AnyWithRole(UserRole role);
        List<User> GetByRole(UserRole Role);
        void Add(User user);
        void Update(User user);
    }
}
