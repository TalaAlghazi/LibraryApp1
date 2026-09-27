using System.Drawing;
using System.Windows.Forms;

namespace LibraryApp1.UI
{
    internal static class UiTheme
    {
        public static readonly Color Background = Color.FromArgb(245, 245, 247);
        public static readonly Color Primary = Color.FromArgb(70, 130, 130);
        public static readonly Color PrimaryDark = Color.FromArgb(50, 95, 95);
        public static readonly Color Danger = Color.FromArgb(205, 92, 92);
        public static readonly Color Success = Color.FromArgb(60, 140, 90);
        public static readonly Color TextMuted = Color.FromArgb(110, 110, 115);
        public static readonly Color InputBorder = Color.FromArgb(200, 205, 210);
        public static readonly Font BaseFont = new("Segoe UI", 10);
        public static readonly Font TitleFont = new("Segoe UI", 18, FontStyle.Bold);

        public static void StyleButton(Button btn, Color backColor, bool fullWidth = false)
        {
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(backColor, 0.08f);
            btn.Cursor = Cursors.Hand;
            btn.Height = 40;
            if (!fullWidth)
                btn.Width = 155;
        }

        public static void StyleTextBox(TextBox box)
        {
            box.Font = BaseFont;
            box.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersHeight = 35;
            grid.RowTemplate.Height = 30;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 248);
            grid.GridColor = Color.FromArgb(220, 230, 235);
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
        }
    }
}