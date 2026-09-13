namespace LibraryApp1.DataAccess
{
    public class SqlBookRepository : IBookRepository
    {
        private readonly LibraryDbContext context;

        public SqlBookRepository(LibraryDbContext context)
        {
            this.context = context;
        }

        public Book? GetById(int id)
        {
            return context.Books.Find(id);
        }

        public List<Book> GetAll(
            int pageNumber,
            int pageSize,
            bool? isAvailable = null,
            string? titleContains = null)
        {
            IQueryable<Book> query = context.Books;

            if (isAvailable.HasValue)
                query = query.Where(b => b.IsAvailable == isAvailable.Value);

            if (!string.IsNullOrWhiteSpace(titleContains))
                query = query.Where(b => b.Title.Contains(titleContains));

            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public void Add(Book book)
        {
            context.Books.Add(book);
            context.SaveChanges();
        }

        public void Update(Book book)
        {
            context.Books.Update(book);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var book = context.Books.Find(id);
            if (book == null) return;

            context.Books.Remove(book);
            context.SaveChanges();
        }
    }
}
