namespace LibraryApp1.DataAccess
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public bool IsAvailable { get; set; }
        public string BorrowerName { get; set; } = "";
        public DateTime DueDate { get; set; }
        public decimal Fine { get; set; } = 0;
    }
}
