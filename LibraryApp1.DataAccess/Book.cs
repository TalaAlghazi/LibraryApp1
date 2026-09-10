namespace LibraryApp1.DataAccess
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public bool IsAvailable { get; set; }

        public virtual int GetLoanPeriodDays() => 14;
        public virtual decimal GetFinePerDay() => 1;
    }
}