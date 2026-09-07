namespace LibraryApp1.DataAccess
{
    public class PrintedBook : Book
    {
        public string ShelfLocation { get; set; } = "";

        public override int GetLoanPeriodDays() => 21;
    }
}
