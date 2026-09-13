using LibraryApp1.BusinessLogic;
using LibraryApp1.DataAccess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace LibraryApp1.UI
{
    public partial class Form1 : Form
    {
        private LibraryDbContext dbContext;
        private IBookRepository bookRepository;
        private IReservationRepository reservationRepository;
        private ILibraryService libraryService;

        public Form1()
        {
            InitializeComponent();

            this.BackColor = Color.FromArgb(245, 245, 247);
            this.Font = new Font("Segoe UI", 10);

            label1.BackColor = Color.FromArgb(70, 130, 130);
            label1.ForeColor = Color.White;
            label1.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            label1.Dock = DockStyle.Top;
            label1.Height = 80;
            label1.TextAlign = ContentAlignment.MiddleLeft;
            label1.Padding = new Padding(20, 0, 0, 0);

            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10);

            textBox1.Font = new Font("Segoe UI", 10);

            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 130);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersHeight = 35;
            dataGridView1.RowTemplate.Height = 30;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 248);
            dataGridView1.GridColor = Color.FromArgb(220, 230, 235);
            dataGridView1.AllowUserToAddRows = false;

            StyleButton(button1, Color.FromArgb(70, 130, 130));
            StyleButton(button2, Color.FromArgb(70, 130, 130));
            StyleButton(button3, Color.FromArgb(70, 130, 130));
            StyleButton(button4, Color.FromArgb(70, 130, 130));
            StyleButton(button5, Color.FromArgb(205, 92, 92));
            StyleButton(button6, Color.FromArgb(70, 130, 130));
            StyleButton(button7, Color.FromArgb(70, 130, 130));
            StyleButton(btnViewReservation, Color.FromArgb(70, 130, 130));
            StyleButton(btnViewFines, Color.FromArgb(70, 130, 130));
       

        dbContext = new LibraryDbContext();
            bookRepository = new SqlBookRepository(dbContext);
            reservationRepository = new SqlReservationRepository(dbContext);
            libraryService = new LibraryService(bookRepository, reservationRepository);
            LoadBooks();
        }

        private void StyleButton(Button btn, Color backColor)
        {
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Height = 40;
            btn.Width = 155;
        }

        private void LoadBooks()
        {
            try
            {
                var books = new List<Book>()
                {
                    new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", IsAvailable = true },
                    new Book { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee", IsAvailable = true },
                    new Book { Id = 3, Title = "1984", Author = "George Orwell", IsAvailable = true },
                    new Book { Id = 4, Title = "Pride and Prejudice", Author = "Jane Austen", IsAvailable = true },
                    new Book { Id = 5, Title = "The Catcher in the Rye", Author = "J.D. Salinger", IsAvailable = true },
                    new Book { Id = 6, Title = "Brave New World", Author = "Aldous Huxley", IsAvailable = true },
                    new Book { Id = 7, Title = "Jane Eyre", Author = "Charlotte Bronte", IsAvailable = true },
                    new Book { Id = 8, Title = "Wuthering Heights", Author = "Emily Bronte", IsAvailable = true }
                };
                dataGridView1.DataSource = books;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadBooks();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string title = textBox1.Text;
            try
            {
                var books = new List<Book>()
                {
                    new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", IsAvailable = true },
                    new Book { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee", IsAvailable = true },
                    new Book { Id = 3, Title = "1984", Author = "George Orwell", IsAvailable = true },
                    new Book { Id = 4, Title = "Pride and Prejudice", Author = "Jane Austen", IsAvailable = true },
                    new Book { Id = 5, Title = "The Catcher in the Rye", Author = "J.D. Salinger", IsAvailable = true },
                    new Book { Id = 6, Title = "Brave New World", Author = "Aldous Huxley", IsAvailable = true },
                    new Book { Id = 7, Title = "Jane Eyre", Author = "Charlotte Bronte", IsAvailable = true },
                    new Book { Id = 8, Title = "Wuthering Heights", Author = "Emily Bronte", IsAvailable = true }
                };

                var filtered = books.Where(b => b.Title.ToLower().Contains(title.ToLower())).ToList();

                if (filtered.Count == 0)
                {
                    MessageBox.Show("No books found!");
                    LoadBooks();
                }
                else
                {
                    dataGridView1.DataSource = filtered;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book first!");
                return;
            }

            try
            {
                int bookId = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                string borrowerName = PromptForInput("Enter borrower name:");

                if (string.IsNullOrWhiteSpace(borrowerName))
                    return;

                var reservation = new Reservation
                {
                    BookId = bookId,
                    BorrowerName = borrowerName,
                    ReservedAt = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(14),
                    ReturnedAt = null,
                    Fine = 0
                };

                reservationRepository.Add(reservation);
                MessageBox.Show("Book reserved successfully!");
                LoadBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book first!");
                return;
            }

            try
            {
                int bookId = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                var reservation = reservationRepository.GetActiveByBookId(bookId);

                if (reservation == null)
                {
                    MessageBox.Show("No active reservation for this book!");
                    return;
                }

                reservation.ReturnedAt = DateTime.Now;
                reservationRepository.Update(reservation);

                var book = bookRepository.GetById(bookId);
                book.IsAvailable = true;
                bookRepository.Update(book);

                MessageBox.Show("Book returned successfully!");
                LoadBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book first!");
                return;
            }

            try
            {
                int bookId = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
                DialogResult result = MessageBox.Show("Are you sure?", "Delete", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    bookRepository.Delete(bookId);
                    MessageBox.Show("Book deleted!");
                    LoadBooks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button4_Click(sender, e);
        }

        private string PromptForInput(string prompt)
        {
            var form = new Form
            {
                Text = prompt,
                Width = 300,
                Height = 150,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var label = new Label { Left = 20, Top = 20, Text = prompt, Width = 250 };
            var textBox = new TextBox { Left = 20, Top = 50, Width = 250 };
            var okButton = new Button { Text = "OK", Left = 120, Width = 80, Top = 80, DialogResult = DialogResult.OK };
            var cancelButton = new Button { Text = "Cancel", Left = 200, Width = 80, Top = 80, DialogResult = DialogResult.Cancel };

            form.Controls.Add(label);
            form.Controls.Add(textBox);
            form.Controls.Add(okButton);
            form.Controls.Add(cancelButton);
            form.AcceptButton = okButton;
            form.CancelButton = cancelButton;

            return form.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void button8_Click(object sender, EventArgs e)
        {
            DisplayActiveReservations();
        }

        private void DisplayActiveReservations()
        {
            try
            {
                var result = libraryService.GetActiveReservations(1, 10);

                if (result.IsSuccess)
                {
                    dataGridView1.DataSource = result.Data;
                    MessageBox.Show("Active Reservations displayed successfully!");
                }
                else
                {
                    MessageBox.Show(result.Description);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ViewFines();
        }

        private void ViewFines()
        {
            try
            {
                var result = libraryService.GetReservationsWithFines(1, 10);

                if (result.IsSuccess)
                {
                    dataGridView1.DataSource = result.Data;
                    MessageBox.Show("Outstanding Fines displayed successfully!");
                }
                else
                {
                    MessageBox.Show(result.Description);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}