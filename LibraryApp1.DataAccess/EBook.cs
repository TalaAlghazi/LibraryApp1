namespace LibraryApp1.DataAccess
{
    public class EBook : Book
    {
        public string DownloadUrl { get; set; } = "";

        public override int GetLoanPeriodDays() => 7;

        public override decimal GetFinePerDay() => 0.5m;
    }
}
