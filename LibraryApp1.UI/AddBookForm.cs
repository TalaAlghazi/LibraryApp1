using System;
using System.Drawing;
using System.Windows.Forms;

namespace LibraryApp1.UI
{
    public class AddBookForm : Form
    {
        private readonly TextBox titleBox = new();
        private readonly TextBox authorBox = new();
        private readonly Label statusLabel = new();
        private readonly Button addButton = new();

        public string BookTitle => titleBox.Text.Trim();
        public string BookAuthor => authorBox.Text.Trim();

        public AddBookForm()
        {
            Text = "Add Book";
            ClientSize = new Size(360, 260);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = UiTheme.Background;
            Font = UiTheme.BaseFont;
            AcceptButton = addButton;

            var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 20, 24, 20) };

            var titleLabel = new Label { Text = "Title", AutoSize = true, Top = 0, Left = 0, ForeColor = UiTheme.TextMuted };
            titleBox.SetBounds(0, 22, 300, 30);
            UiTheme.StyleTextBox(titleBox);

            var authorLabel = new Label { Text = "Author", AutoSize = true, Top = 64, Left = 0, ForeColor = UiTheme.TextMuted };
            authorBox.SetBounds(0, 86, 300, 30);
            UiTheme.StyleTextBox(authorBox);

            statusLabel.SetBounds(0, 124, 300, 30);
            statusLabel.ForeColor = UiTheme.Danger;

            addButton.SetBounds(0, 164, 300, 40);
            addButton.Text = "Add Book";
            UiTheme.StyleButton(addButton, UiTheme.Primary, fullWidth: true);
            addButton.Click += AddButton_Click;

            body.Controls.Add(titleLabel);
            body.Controls.Add(titleBox);
            body.Controls.Add(authorLabel);
            body.Controls.Add(authorBox);
            body.Controls.Add(statusLabel);
            body.Controls.Add(addButton);

            Controls.Add(body);
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BookTitle) || string.IsNullOrWhiteSpace(BookAuthor))
            {
                statusLabel.Text = "Please enter both a title and an author.";
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}