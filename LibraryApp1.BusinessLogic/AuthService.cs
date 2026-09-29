using System.Text.RegularExpressions;
using LibraryApp1.DataAccess;

namespace LibraryApp1.BusinessLogic
{
    public class AuthService : IAuthService
    {
        private static readonly Regex EmailPattern =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private static readonly Regex UsernamePattern =
            new(@"^[A-Za-z0-9_.-]+$", RegexOptions.Compiled);

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

            var error = ValidateUsername(username) ?? ValidateEmail(email) ?? ValidateNewPassword(password, confirmPassword);
            if (error != null)
                return Result<AuthenticatedUser>.Failure(400, error);

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

        public Result<AuthenticatedUser> UpdateProfile(int userId, string username, string email)
        {
            var user = userRepository.GetById(userId);
            if (user == null)
                return Result<AuthenticatedUser>.Failure(404, "Account not found.");

            username = (username ?? "").Trim();
            email = (email ?? "").Trim();

            var error = ValidateUsername(username) ?? ValidateEmail(email);
            if (error != null)
                return Result<AuthenticatedUser>.Failure(400, error);

            var sameUsername = userRepository.GetByUsername(username);
            if (sameUsername != null && sameUsername.Id != userId)
                return Result<AuthenticatedUser>.Failure(409, "That username is already taken.");

            var sameEmail = userRepository.GetByEmail(email);
            if (sameEmail != null && sameEmail.Id != userId)
                return Result<AuthenticatedUser>.Failure(409, "An account with that email already exists.");

            user.Username = username;
            user.Email = email;
            userRepository.Update(user);

            return Result<AuthenticatedUser>.Success(ToAuthenticatedUser(user), "Profile updated successfully.");
        }

        public Result<string> ChangePassword(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = userRepository.GetById(userId);
            if (user == null)
                return Result<string>.Failure(404, "Account not found.");

            if (!PasswordHasher.Verify(currentPassword ?? "", user.PasswordHash))
                return Result<string>.Failure(400, "Current password is incorrect.");

            var error = ValidateNewPassword(newPassword ?? "", confirmPassword);
            if (error != null)
                return Result<string>.Failure(400, error);

            user.PasswordHash = PasswordHasher.Hash(newPassword!);
            userRepository.Update(user);

            return Result<string>.Success("Password changed successfully.");
        }

        private static string? ValidateUsername(string username)
        {
            if (username.Length < 3 || username.Length > 50)
                return "Username must be between 3 and 50 characters.";

            if (!UsernamePattern.IsMatch(username))
                return "Username may only contain letters, digits, '.', '_' and '-'.";

            return null;
        }

        private static string? ValidateEmail(string email)
        {
            return EmailPattern.IsMatch(email) ? null : "Please provide a valid email address.";
        }

        private static string? ValidateNewPassword(string password, string confirmPassword)
        {
            if (password.Length < 8 || !password.Any(char.IsDigit) || !password.Any(char.IsLetter))
                return "Password must be at least 8 characters and include a letter and a digit.";

            if (password != confirmPassword)
                return "Passwords do not match.";

            return null;
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
