namespace LibraryApp1.UI
{
    partial class Form1
    {
        
        private System.ComponentModel.IContainer components = null;

       
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code


        private void InitializeComponent()
        {
            label1 = new Label();
            dataGridView1 = new DataGridView();
            Id = new DataGridViewTextBoxColumn();
            Title = new DataGridViewTextBoxColumn();
            Author = new DataGridViewTextBoxColumn();
            IsAvailable = new DataGridViewTextBoxColumn();
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
            label1.Text = "Available Books";
            label1.Click += label1_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { Id, Title, Author, IsAvailable });
            dataGridView1.Location = new Point(21, 151);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(945, 225);
            dataGridView1.TabIndex = 1;
            // 
            // Id
            // 
            Id.DataPropertyName = "Id";
            Id.HeaderText = "Column1";
            Id.MinimumWidth = 8;
            Id.Name = "Id";
            Id.Width = 150;
            // 
            // Title
            // 
            Title.DataPropertyName = "Title";
            Title.HeaderText = "Column1";
            Title.MinimumWidth = 8;
            Title.Name = "Title";
            Title.Width = 150;
            // 
            // Author
            // 
            Author.DataPropertyName = "Author";
            Author.HeaderText = "Column1";
            Author.MinimumWidth = 8;
            Author.Name = "Author";
            Author.Width = 150;
            // 
            // IsAvailable
            // 
            IsAvailable.DataPropertyName = "IsAvailable";
            IsAvailable.HeaderText = "Column1";
            IsAvailable.MinimumWidth = 8;
            IsAvailable.Name = "IsAvailable";
            IsAvailable.Width = 150;
            // 
            // button1
            // 
            button1.Location = new Point(72, 423);
            button1.Name = "button1";
            button1.Size = new Size(174, 34);
            button1.TabIndex = 2;
            button1.Text = "Refresh";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // button2
            // 
            button2.Location = new Point(290, 423);
            button2.Name = "button2";
            button2.Size = new Size(170, 34);
            button2.TabIndex = 3;
            button2.Text = "Reserve";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(290, 498);
            button3.Name = "button3";
            button3.Size = new Size(170, 34);
            button3.TabIndex = 4;
            button3.Text = "Return";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(510, 423);
            button4.Name = "button4";
            button4.Size = new Size(170, 34);
            button4.TabIndex = 5;
            button4.Text = "Search";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Location = new Point(736, 423);
            button5.Name = "button5";
            button5.Size = new Size(170, 34);
            button5.TabIndex = 6;
            button5.Text = "Delete";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Location = new Point(736, 498);
            button6.Name = "button6";
            button6.Size = new Size(170, 34);
            button6.TabIndex = 7;
            button6.Text = "Exit";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(54, 96);
            label2.Name = "label2";
            label2.Size = new Size(64, 25);
            label2.TabIndex = 8;
            label2.Text = "Search";
            label2.Click += label2_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(178, 90);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(670, 31);
            textBox1.TabIndex = 9;
            textBox1.Text = "TextBox";
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // button7
            // 
            button7.Location = new Point(854, 87);
            button7.Name = "button7";
            button7.Size = new Size(112, 34);
            button7.TabIndex = 10;
            button7.Text = "Search";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // btnViewReservation
            // 
            btnViewReservation.BackColor = Color.Teal;
            btnViewReservation.Location = new Point(72, 498);
            btnViewReservation.Name = "btnViewReservation";
            btnViewReservation.Size = new Size(174, 34);
            btnViewReservation.TabIndex = 11;
            btnViewReservation.Text = "Reservations";
            btnViewReservation.UseVisualStyleBackColor = true;
            btnViewReservation.Click += button8_Click;
            // 
            // btnViewFines
            // 
            btnViewFines.BackColor = Color.Teal;
            btnViewFines.Location = new Point(510, 498);
            btnViewFines.Name = "btnViewFines";
            btnViewFines.Size = new Size(170, 34);
            btnViewFines.TabIndex = 12;
            btnViewFines.Text = "View Fines";
            btnViewFines.UseVisualStyleBackColor = true;
            btnViewFines.Click += button9_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(978, 544);
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
            Controls.Add(dataGridView1);
            Controls.Add(label1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Library Manegment System";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private DataGridView dataGridView1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Label label2;
        private TextBox textBox1;
        private Button button7;
        private DataGridViewTextBoxColumn Id;
        private DataGridViewTextBoxColumn Title;
        private DataGridViewTextBoxColumn Author;
        private DataGridViewTextBoxColumn IsAvailable;
        private Button btnViewReservation;
        private Button btnViewFines;
    }
}
