using System.Collections.Specialized;

namespace LibraryApp1.DataAccess
{
    public class Reservation
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int? UserId { get; set; }
        public string BorrowerName { get; set; } = "";
        public string? BorrowerPhone { get; set; }
        public DateTime ReservedAt { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnRequestedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public decimal Fine { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Active;
        public bool IsActive => ReturnedAt == null;
        public virtual Book? Book { get; set; }
        public virtual User? User { get; set; }
    }
}
