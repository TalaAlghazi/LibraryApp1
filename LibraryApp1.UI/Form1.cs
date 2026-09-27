using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LibraryApp1.UI
{
    public partial class Form1 : Form
    {
        private readonly LibraryDbContext dbContext;
        private readonly IBookRepository bookRepository;
        private readonly IReservationRepository reservationRepository;
        private readonly ILibraryService libraryService;
        private readonly AuthenticatedUser currentUser;

        public bool LogoutRequested { get; private set; }

        public Form1(AuthenticatedUser currentUser)
        {
            this.currentUser = currentUser;

            InitializeComponent();

            BackColor = UiTheme.Background;
            Font = UiTheme.BaseFont;

            label1.BackColor = UiTheme.Primary;
            label1.ForeColor = Color.White;
            label1.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            label1.Dock = DockStyle.Top;
            label1.Height = 80;
            label1.TextAlign = ContentAlignment.MiddleLeft;
            label1.Padding = new Padding(20, 0, 0, 0);

            userInfoLabel.AutoSize = true;
            userInfoLabel.Font = new Font("Segoe UI", 9.5f, FontStyle.Italic);
            userInfoLabel.ForeColor = UiTheme.TextMuted;
            userInfoLabel.Text = $"Signed in as {currentUser.Username} ({currentUser.Role})";

            UiTheme.StyleButton(logoutButton, UiTheme.Danger);

            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10);

            UiTheme.StyleTextBox(textBox1);

            UiTheme.StyleGrid(dataGridView1);

            emptyStateLabel.Font = new Font("Segoe UI", 11, FontStyle.Italic);
            emptyStateLabel.ForeColor = UiTheme.TextMuted;

            UiTheme.StyleButton(button1, UiTheme.Primary);
            UiTheme.StyleButton(button2, UiTheme.Primary);
            UiTheme.StyleButton(button3, UiTheme.Primary);
            UiTheme.StyleButton(button4, UiTheme.Primary);
            UiTheme.StyleButton(button5, UiTheme.Danger);
            UiTheme.StyleButton(button6, UiTheme.Primary);
            UiTheme.StyleButton(button7, UiTheme.Primary);
            UiTheme.StyleButton(btnViewReservation, UiTheme.Primary);
            UiTheme.StyleButton(btnViewFines, UiTheme.Primary);

            bool isAdmin = currentUser.Role == UserRole.Admin;
            button4.Visible = isAdmin;
            button5.Visible = isAdmin;

            dbContext = new LibraryDbContext();
            bookRepository = new SqlBookRepository(dbContext);
            reservationRepository = new SqlReservationRepository(dbContext);
            libraryService = new LibraryService(bookRepository, reservationRepository);

            LoadBooks();
        }

        private void ShowGrid<T>(List<T> data, string emptyMessage)
        {
            dataGridView1.DataSource = data;

            bool hasData = data.Count > 0;
            dataGridView1.Visible = hasData;
            emptyStateLabel.Visible = !hasData;
            emptyStateLabel.Text = emptyMessage;
        }

        private void RunWithLoadingCursor(Action action)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                action();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void LoadBooks()
        {
            RunWithLoadingCursor(() =>
            {
                try
                {
                    var result = libraryService.GetAvailableBooks(1, 50);
                    if (result.IsSuccess)
                    {
                        ShowGrid(result.Data!, "No available books. Try adding one or check back later.");
                        ConfigureBookColumns();
                    }
                    else
                    {
                        MessageBox.Show(this, result.Description, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Could not load books: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void ConfigureBookColumns()
        {
            if (dataGridView1.Columns["Id"] is { } idCol) idCol.HeaderText = "ID";
            if (dataGridView1.Columns["Title"] is { } titleCol) titleCol.HeaderText = "Title";
            if (dataGridView1.Columns["Author"] is { } authorCol) authorCol.HeaderText = "Author";
            if (dataGridView1.Columns["IsAvailable"] is { } availCol) availCol.HeaderText = "Available";
        }

        private void ConfigureReservationColumns()
        {
            SetHeader("ReservationId", "Reservation #");
            SetHeader("BookId", "Book ID");
            SetHeader("BookTitle", "Book");
            SetHeader("BorrowerName", "Borrower");
            SetHeader("BorrowerPhone", "Phone");
            SetHeader("ReservedAt", "Reserved On");
            SetHeader("DueDate", "Due Date");
            SetHeader("ReturnedAt", "Returned On");
            SetHeader("Fine", "Fine ($)");
            SetHeader("IsActive", "Active?");

            void SetHeader(string columnName, string headerText)
            {
                if (dataGridView1.Columns[columnName] is { } col)
                    col.HeaderText = headerText;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadBooks();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (currentUser.Role != UserRole.Admin)
            {
                MessageBox.Show(this, "Only administrators can add books.", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var form = new AddBookForm();
            if (form.ShowDialog(this) != DialogResult.OK)
                return;

            RunWithLoadingCursor(() =>
            {
                try
                {
                    var result = libraryService.AddBook(form.BookTitle, form.BookAuthor, currentUser.Role);
                    if (result.IsSuccess)
                    {
                        MessageBox.Show(this, "Book added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadBooks();
                    }
                    else
                    {
                        MessageBox.Show(this, result.Description, "Could not add book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "Please select a book first!", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int bookId = (int)dataGridView1.SelectedRows[0].Cells["Id"].Value;
                string? borrowerName = PromptForInput("Enter borrower name:");
                if (string.IsNullOrWhiteSpace(borrowerName))
                    return;

                string? borrowerPhone = PromptForInput("Enter borrower phone (optional):") ?? "";

                RunWithLoadingCursor(() =>
                {
                    var result = libraryService.ReserveBook(bookId, borrowerName, borrowerPhone);
                    if (result.IsSuccess)
                    {
                        MessageBox.Show(this, result.Data, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadBooks();
                    }
                    else
                    {
                        MessageBox.Show(this, result.Description, "Could not reserve book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "Please select a book first!", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int bookId = (int)dataGridView1.SelectedRows[0].Cells["Id"].Value;

                RunWithLoadingCursor(() =>
                {
                    var result = libraryService.ReturnBook(bookId);
                    if (result.IsSuccess)
                    {
                        MessageBox.Show(this, result.Data, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadBooks();
                    }
                    else
                    {
                        MessageBox.Show(this, result.Description, "Could not return book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (currentUser.Role != UserRole.Admin)
            {
                MessageBox.Show(this, "Only administrators can delete books.", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "Please select a book first!", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int bookId = (int)dataGridView1.SelectedRows[0].Cells["Id"].Value;
                var confirm = MessageBox.Show(this, "Are you sure you want to delete this book?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes)
                    return;

                RunWithLoadingCursor(() =>
                {
                    var result = libraryService.DeleteBook(bookId, currentUser.Role);
                    if (result.IsSuccess)
                    {
                        MessageBox.Show(this, result.Data, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadBooks();
                    }
                    else
                    {
                        MessageBox.Show(this, result.Description, "Could not delete book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(this, "Are you sure you want to log out?", "Confirm logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            LogoutRequested = true;
            Close();
        }

        private void SearchBooks()
        {
            RunWithLoadingCursor(() =>
            {
                try
                {
                    var result = libraryService.SearchBook(textBox1.Text.Trim(), 1, 50);
                    if (result.IsSuccess)
                    {
                        ShowGrid(result.Data!, "No books found matching your search.");
                        ConfigureBookColumns();
                    }
                    else
                    {
                        MessageBox.Show(this, result.Description, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SearchBooks();
        }

        private string? PromptForInput(string prompt)
        {
            var form = new Form
            {
                Text = prompt,
                Width = 340,
                Height = 160,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = UiTheme.Background,
                Font = UiTheme.BaseFont
            };

            var label = new Label { Left = 20, Top = 20, Text = prompt, Width = 280, AutoSize = true };
            var textBox = new TextBox { Left = 20, Top = 48, Width = 280 };
            UiTheme.StyleTextBox(textBox);
            var okButton = new Button { Text = "OK", Left = 130, Width = 80, Top = 84, DialogResult = DialogResult.OK };
            UiTheme.StyleButton(okButton, UiTheme.Primary, fullWidth: true);
            var cancelButton = new Button { Text = "Cancel", Left = 220, Width = 80, Top = 84, DialogResult = DialogResult.Cancel };

            form.Controls.Add(label);
            form.Controls.Add(textBox);
            form.Controls.Add(okButton);
            form.Controls.Add(cancelButton);
            form.AcceptButton = okButton;
            form.CancelButton = cancelButton;

            return form.ShowDialog(this) == DialogResult.OK ? textBox.Text : null;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DisplayActiveReservations();
        }

        private void DisplayActiveReservations()
        {
            RunWithLoadingCursor(() =>
            {
                try
                {
                    var result = libraryService.GetActiveReservations(1, 50);

                    if (result.IsSuccess)
                    {
                        ShowGrid(result.Data!, "No active reservations right now.");
                        ConfigureReservationColumns();
                    }
                    else
                    {
                        MessageBox.Show(this, result.Description, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ViewFines();
        }

        private void ViewFines()
        {
            RunWithLoadingCursor(() =>
            {
                try
                {
                    var result = libraryService.GetReservationsWithFines(1, 50);

                    if (result.IsSuccess)
                    {
                        ShowGrid(result.Data!, "No outstanding fines.");
                        ConfigureReservationColumns();
                    }
                    else
                    {
                        MessageBox.Show(this, result.Description, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }
    }
}