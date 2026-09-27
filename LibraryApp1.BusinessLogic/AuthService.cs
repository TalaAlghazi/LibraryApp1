using System.Text.RegularExpressions;
using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public class AuthService : IAuthService
    {
        private static readonly Regex EmailPattern =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private readonly IUserRepository userRepository;

        public AuthService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public Result<AuthenticatedUser> Register(string username, string email, string password, string confirmPassword)
        {
            username = (username ?? "").Trim();
            email = (email ?? "").Trim();
            password ??= "";

            if (username.Length < 3 || username.Length > 50)
                return Result<AuthenticatedUser>.Failure(400, "Username must be between 3 and 50 characters.");

            if (!Regex.IsMatch(username, @"^[A-Za-z0-9_.-]+$"))
                return Result<AuthenticatedUser>.Failure(400, "Username may only contain letters, digits, '.', '_' and '-'.");

            if (!EmailPattern.IsMatch(email))
                return Result<AuthenticatedUser>.Failure(400, "Please provide a valid email address.");

            if (password.Length < 8 || !password.Any(char.IsDigit) || !password.Any(char.IsLetter))
                return Result<AuthenticatedUser>.Failure(400, "Password must be at least 8 characters and include a letter and a digit.");

            if (password != confirmPassword)
                return Result<AuthenticatedUser>.Failure(400, "Passwords do not match.");

            if (userRepository.GetByUsername(username) != null)
                return Result<AuthenticatedUser>.Failure(409, "That username is already taken.");

            if (userRepository.GetByEmail(email) != null)
                return Result<AuthenticatedUser>.Failure(409, "An account with that email already exists.");

            var role = userRepository.Count() == 0 ? UserRole.Admin : UserRole.Librarian;

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = PasswordHasher.Hash(password),
                Role = role,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            userRepository.Add(user);

            return Result<AuthenticatedUser>.Success(ToAuthenticatedUser(user), "Account created successfully.");
        }

        public Result<AuthenticatedUser> Login(string username, string password)
        {
            username = (username ?? "").Trim();
            password ??= "";

            if (username.Length == 0 || password.Length == 0)
                return Result<AuthenticatedUser>.Failure(400, "Username and password are required.");

            var user = userRepository.GetByUsername(username);

            if (user == null || !user.IsActive || !PasswordHasher.Verify(password, user.PasswordHash))
                return Result<AuthenticatedUser>.Failure(401, "Invalid username or password.");

            return Result<AuthenticatedUser>.Success(ToAuthenticatedUser(user), "Login successful.");
        }

        private static AuthenticatedUser ToAuthenticatedUser(User user) => new AuthenticatedUser
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
    }
}
