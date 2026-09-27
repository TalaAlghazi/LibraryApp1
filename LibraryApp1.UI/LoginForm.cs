using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;

namespace LibraryApp1.UI
{
    public class LoginForm : Form
    {
        private readonly TextBox usernameBox = new();
        private readonly TextBox passwordBox = new();
        private readonly Label statusLabel = new();
        private readonly Button loginButton = new();
        private readonly Button registerLinkButton = new();
        private readonly LibraryDbContext dbContext;
        private readonly IAuthService authService;

        public AuthenticatedUser? LoggedInUser { get; private set; }

        public LoginForm()
        {
            dbContext = new LibraryDbContext();
            authService = new AuthService(new SqlUserRepository(dbContext));

            BuildUi();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }
            base.Dispose(disposing);
        }

        private void BuildUi()
        {
            Text = "Library Management System - Sign In";
            ClientSize = new Size(420, 420);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = UiTheme.Background;
            Font = UiTheme.BaseFont;
            AcceptButton = loginButton;

            const int FormWidth = 420;
            const int FieldWidth = 330;
            int fieldLeft = (FormWidth - FieldWidth) / 2;

            var header = new Label
            {
                Text = "Library",
                ForeColor = Color.White,
                BackColor = UiTheme.Primary,
                Dock = DockStyle.Top,
                Height = 90,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(24, 0, 0, 0),
                Font = UiTheme.TitleFont
            };

            var body = new Panel { Dock = DockStyle.Fill };

            var usernameLabel = new Label { Text = "Username", AutoSize = true, Top = 10, Left = fieldLeft, ForeColor = UiTheme.TextMuted };
            usernameBox.SetBounds(fieldLeft, 36, FieldWidth, 30);
            UiTheme.StyleTextBox(usernameBox);

            var passwordLabel = new Label { Text = "Password", AutoSize = true, Top = 82, Left = fieldLeft, ForeColor = UiTheme.TextMuted };
            passwordBox.SetBounds(fieldLeft, 108, FieldWidth, 30);
            passwordBox.UseSystemPasswordChar = true;
            UiTheme.StyleTextBox(passwordBox);

            statusLabel.SetBounds(fieldLeft, 146, FieldWidth, 30);
            statusLabel.ForeColor = UiTheme.Danger;
            statusLabel.Text = "";

            loginButton.SetBounds(fieldLeft, 182, FieldWidth, 42);
            loginButton.Text = "Sign In";
            UiTheme.StyleButton(loginButton, UiTheme.Primary, fullWidth: true);
            loginButton.Click += LoginButton_Click;

            var hintLabel = new Label
            {
                Text = "New here?",
                ForeColor = UiTheme.TextMuted,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 232,
                Left = fieldLeft,
                Width = FieldWidth,
                Height = 22
            };

            registerLinkButton.SetBounds(fieldLeft, 258, FieldWidth, 34);
            registerLinkButton.Text = "Create a new account";
            registerLinkButton.FlatStyle = FlatStyle.Flat;
            registerLinkButton.FlatAppearance.BorderSize = 0;
            registerLinkButton.BackColor = UiTheme.Background;
            registerLinkButton.ForeColor = UiTheme.Primary;
            registerLinkButton.Cursor = Cursors.Hand;
            registerLinkButton.TextAlign = ContentAlignment.MiddleCenter;
            registerLinkButton.Click += RegisterLinkButton_Click;

            body.Controls.Add(usernameLabel);
            body.Controls.Add(usernameBox);
            body.Controls.Add(passwordLabel);
            body.Controls.Add(passwordBox);
            body.Controls.Add(statusLabel);
            body.Controls.Add(loginButton);
            body.Controls.Add(hintLabel);
            body.Controls.Add(registerLinkButton);

            Controls.Add(body);
            Controls.Add(header);
        }
        private void LoginButton_Click(object? sender, EventArgs e)
        {
            statusLabel.Text = "";

            var username = usernameBox.Text.Trim();
            var password = passwordBox.Text;

            if (username.Length == 0 || password.Length == 0)
            {
                statusLabel.Text = "Please enter both username and password.";
                return;
            }

            SetLoading(true);
            try
            {
                var result = authService.Login(username, password);
                if (!result.IsSuccess)
                {
                    statusLabel.Text = result.Description;
                    return;
                }

                LoggedInUser = result.Data;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Could not connect to the database. " + ex.Message;
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void RegisterLinkButton_Click(object? sender, EventArgs e)
        {
            using var registerForm = new RegisterForm();
            if (registerForm.ShowDialog(this) == DialogResult.OK && registerForm.RegisteredUser != null)
            {
                LoggedInUser = registerForm.RegisteredUser;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // LoginForm
            // 
            ClientSize = new Size(385, 501);
            Name = "LoginForm";
            Load += LoginForm_Load;
            ResumeLayout(false);

        }

        private void SetLoading(bool isLoading)
        {
            Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
            loginButton.Enabled = !isLoading;
            registerLinkButton.Enabled = !isLoading;
            loginButton.Text = isLoading ? "Signing in..." : "Sign In";
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}