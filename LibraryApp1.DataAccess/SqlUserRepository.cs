using Microsoft.EntityFrameworkCore;

namespace LibraryApp1.DataAccess
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly LibraryDbContext context;

        public SqlUserRepository(LibraryDbContext context)
        {
            this.context = context;
        }

        public User? GetById(int id)
        {
            return context.Users.Find(id);
        }

        public User? GetByUsername(string username)
        {
            return context.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
        }

        public User? GetByEmail(string email)
        {
            return context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        }

        public int Count()
        {
            return context.Users.Count();
        }
        public bool AnyWithRole(UserRole role)
        {
            return context.Users.Any(u => u.Role == role);
        }

        public List<User> GetByRole(UserRole role)
        {
            return context.Users
                .Where(u => u.Role == role)
                .OrderBy(u => u.Username)
                .ToList();
        }

        public void Add(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }

        public void Update(User user)
        {
            context.Users.Update(user);
            context.SaveChanges();
        }
    }
}