namespace LibraryApp1.UI
{
    internal static class namespaceUI
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            while (true)
            {
                using var loginForm = new LoginForm();
                if (loginForm.ShowDialog() != DialogResult.OK || loginForm.LoggedInUser == null)
                    return;

                using var mainForm = new Form1(loginForm.LoggedInUser);
                Application.Run(mainForm);

                if (!mainForm.LogoutRequested)
                    return;
            }
        }
    }
}