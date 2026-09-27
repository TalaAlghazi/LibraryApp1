using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;

namespace LibraryApp1.UI
{
    public class RegisterForm : Form
    {
        private readonly TextBox usernameBox = new();
        private readonly TextBox emailBox = new();
        private readonly TextBox passwordBox = new();
        private readonly TextBox confirmPasswordBox = new();
        private readonly Label statusLabel = new();
        private readonly Button registerButton = new();
        private readonly LibraryDbContext dbContext;
        private readonly IAuthService authService;

        public AuthenticatedUser? RegisteredUser { get; private set; }

        public RegisterForm()
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
            Text = "Create Account";
            ClientSize = new Size(440, 540);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = UiTheme.Background;
            Font = UiTheme.BaseFont;
            AcceptButton = registerButton;

            const int FormWidth = 440;
            const int FieldWidth = 360;
            int fieldLeft = (FormWidth - FieldWidth) / 2;

            var header = new Label
            {
                Text = "Create Account",
                ForeColor = Color.White,
                BackColor = UiTheme.Primary,
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(24, 0, 0, 0),
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };

            var body = new Panel { Dock = DockStyle.Fill };

            AddField(body, "Username", usernameBox, 0, fieldLeft, FieldWidth);
            AddField(body, "Email", emailBox, 74, fieldLeft, FieldWidth);
            AddField(body, "Password", passwordBox, 148, fieldLeft, FieldWidth, isPassword: true);
            AddField(body, "Confirm password", confirmPasswordBox, 222, fieldLeft, FieldWidth, isPassword: true);

            statusLabel.SetBounds(fieldLeft, 296, FieldWidth, 40);
            statusLabel.ForeColor = UiTheme.Danger;
            statusLabel.Text = "";

            registerButton.SetBounds(fieldLeft, 344, FieldWidth, 44);
            registerButton.Text = "Create Account";
            UiTheme.StyleButton(registerButton, UiTheme.Primary, fullWidth: true);
            registerButton.Click += RegisterButton_Click;

            var rulesLabel = new Label
            {
                Text = "Password must be at least 8 characters and include a letter and a digit.",
                ForeColor = UiTheme.TextMuted,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Top = 396,
                Left = fieldLeft,
                Width = FieldWidth,
                Height = 40
            };

            body.Controls.Add(statusLabel);
            body.Controls.Add(registerButton);
            body.Controls.Add(rulesLabel);

            Controls.Add(body);
            Controls.Add(header);
        }

        private static void AddField(Panel body, string labelText, TextBox box, int top, int left, int width, bool isPassword = false)
        {
            var label = new Label { Text = labelText, AutoSize = true, Top = top, Left = left, ForeColor = UiTheme.TextMuted };
            box.SetBounds(left, top + 28, width, 32);
            box.UseSystemPasswordChar = isPassword;
            UiTheme.StyleTextBox(box);
            body.Controls.Add(label);
            body.Controls.Add(box);
        }

        private void RegisterButton_Click(object? sender, EventArgs e)
        {
            statusLabel.Text = "";

            SetLoading(true);
            try
            {
                var result = authService.Register(
                    usernameBox.Text.Trim(),
                    emailBox.Text.Trim(),
                    passwordBox.Text,
                    confirmPasswordBox.Text);

                if (!result.IsSuccess)
                {
                    statusLabel.Text = result.Description;
                    return;
                }

                RegisteredUser = result.Data;
                MessageBox.Show(
                    this,
                    $"Welcome, {result.Data!.Username}! Your account was created as {result.Data.Role}.",
                    "Account created",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

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

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // RegisterForm
            // 
            ClientSize = new Size(299, 410);
            Name = "RegisterForm";
            Load += RegisterForm_Load;
            ResumeLayout(false);

        }

        private void SetLoading(bool isLoading)
        {
            Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
            registerButton.Enabled = !isLoading;
            registerButton.Text = isLoading ? "Creating account..." : "Create Account";
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }
    }
}                                
