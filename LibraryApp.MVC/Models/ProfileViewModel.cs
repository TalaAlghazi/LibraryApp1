namespace LibraryApp.MVC.Models
{
    public class ProfileViewModel
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public DateTime MemberSince { get; set; }
    }
}