namespace LibraryApp1.DataAccess
{
    public interface IBookRepository
    {
        Book? GetById(int id);

        List<Book> GetAll(
            int pageNumber,
            int pageSize,
            bool? isAvailable = null,
            bool? hasFine = null,
            string? titleContains = null);

        void Add(Book book);
        void Update(Book book);
        void Delete(int id);
    }
}