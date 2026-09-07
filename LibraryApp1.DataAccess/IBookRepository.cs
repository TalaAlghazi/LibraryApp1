namespace LibraryApp1.DataAccess
{
    public interface IBookRepository
    {
        List<Book> LoadBooks();
        void SaveBooks(List<Book> books);
    }
}