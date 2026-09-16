namespace LibraryApp1.DataAccess
{
    public class ReservationWithBookDto
    {
        public int ReservationId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = "";
        public string BorrowerName { get; set; } = "";
        public string? BorrowerPhone { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public decimal Fine { get; set; }
        public bool IsActive => ReturnedAt == null;
    }
}