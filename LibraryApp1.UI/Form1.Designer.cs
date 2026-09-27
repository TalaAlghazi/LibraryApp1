namespace LibraryApp1.UI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing)
            {
                dbContext?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            label1 = new Label();
            userInfoLabel = new Label();
            logoutButton = new Button();
            dataGridView1 = new DataGridView();
            emptyStateLabel = new Label();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            label2 = new Label();
            textBox1 = new TextBox();
            button7 = new Button();
            btnViewReservation = new Button();
            btnViewFines = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            //
            // label1
            //
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(21, 20);
            label1.Name = "label1";
            label1.Size = new Size(225, 38);
            label1.TabIndex = 0;
            label1.Text = "Library Management System";
            //
            // userInfoLabel
            //
            userInfoLabel.AutoSize = true;
            userInfoLabel.Location = new Point(24, 92);
            userInfoLabel.Name = "userInfoLabel";
            userInfoLabel.Size = new Size(140, 25);
            userInfoLabel.TabIndex = 13;
            userInfoLabel.Text = "Signed in as ...";
            //
            // logoutButton
            //
            logoutButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logoutButton.Location = new Point(830, 84);
            logoutButton.Name = "logoutButton";
            logoutButton.Size = new Size(146, 36);
            logoutButton.TabIndex = 14;
            logoutButton.Text = "Logout";
            logoutButton.UseVisualStyleBackColor = true;
            logoutButton.Click += logoutButton_Click;
            //
            // dataGridView1
            //
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Location = new Point(24, 180);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(945, 330);
            dataGridView1.TabIndex = 1;
            //
            // emptyStateLabel
            //
            emptyStateLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            emptyStateLabel.BackColor = Color.White;
            emptyStateLabel.Location = new Point(24, 180);
            emptyStateLabel.Name = "emptyStateLabel";
            emptyStateLabel.Size = new Size(945, 330);
            emptyStateLabel.TabIndex = 15;
            emptyStateLabel.Text = "No records to show.";
            emptyStateLabel.TextAlign = ContentAlignment.MiddleCenter;
            emptyStateLabel.Visible = false;
            //
            // button1
            //
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button1.Location = new Point(24, 526);
            button1.Name = "button1";
            button1.Size = new Size(174, 40);
            button1.TabIndex = 2;
            button1.Text = "Refresh";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            //
            // button2
            //
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button2.Location = new Point(242, 526);
            button2.Name = "button2";
            button2.Size = new Size(170, 40);
            button2.TabIndex = 3;
            button2.Text = "Reserve";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            //
            // button3
            //
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button3.Location = new Point(242, 576);
            button3.Name = "button3";
            button3.Size = new Size(170, 40);
            button3.TabIndex = 4;
            button3.Text = "Return";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            //
            // button4
            //
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button4.Location = new Point(460, 526);
            button4.Name = "button4";
            button4.Size = new Size(170, 40);
            button4.TabIndex = 5;
            button4.Text = "Add Book";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            //
            // button5
            //
            button5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button5.Location = new Point(678, 526);
            button5.Name = "button5";
            button5.Size = new Size(170, 40);
            button5.TabIndex = 6;
            button5.Text = "Delete";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            //
            // button6
            //
            button6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button6.Location = new Point(678, 576);
            button6.Name = "button6";
            button6.Size = new Size(170, 40);
            button6.TabIndex = 7;
            button6.Text = "Exit";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            //
            // label2
            //
            label2.AutoSize = true;
            label2.Location = new Point(24, 138);
            label2.Name = "label2";
            label2.Size = new Size(64, 25);
            label2.TabIndex = 8;
            label2.Text = "Search";
            //
            // textBox1
            //
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Location = new Point(100, 132);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Search by title...";
            textBox1.Size = new Size(650, 31);
            textBox1.TabIndex = 9;
            //
            // button7
            //
            button7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button7.Location = new Point(764, 129);
            button7.Name = "button7";
            button7.Size = new Size(112, 34);
            button7.TabIndex = 10;
            button7.Text = "Search";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            //
            // btnViewReservation
            //
            btnViewReservation.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnViewReservation.BackColor = Color.Teal;
            btnViewReservation.Location = new Point(24, 576);
            btnViewReservation.Name = "btnViewReservation";
            btnViewReservation.Size = new Size(174, 40);
            btnViewReservation.TabIndex = 11;
            btnViewReservation.Text = "Reservations";
            btnViewReservation.UseVisualStyleBackColor = true;
            btnViewReservation.Click += button8_Click;
            //
            // btnViewFines
            //
            btnViewFines.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnViewFines.BackColor = Color.Teal;
            btnViewFines.Location = new Point(460, 576);
            btnViewFines.Name = "btnViewFines";
            btnViewFines.Size = new Size(170, 40);
            btnViewFines.TabIndex = 12;
            btnViewFines.Text = "View Fines";
            btnViewFines.UseVisualStyleBackColor = true;
            btnViewFines.Click += button9_Click;
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(994, 646);
            MinimumSize = new Size(860, 560);
            Controls.Add(btnViewFines);
            Controls.Add(btnViewReservation);
            Controls.Add(button7);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(emptyStateLabel);
            Controls.Add(dataGridView1);
            Controls.Add(logoutButton);
            Controls.Add(userInfoLabel);
            Controls.Add(label1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Library Management System";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label userInfoLabel;
        private Button logoutButton;
        private DataGridView dataGridView1;
        private Label emptyStateLabel;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Label label2;
        private TextBox textBox1;
        private Button button7;
        private Button btnViewReservation;
        private Button btnViewFines;
    }
}
