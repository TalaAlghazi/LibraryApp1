using System.Text.Json;

namespace LibraryApp1.DataAccess
{
    public class FileBookRepository : IBookRepository
    {
        private readonly string filePath = "books.json";

        private List<Book> ReadFile()
        {
            if (!File.Exists(filePath))
            {
                var seed = GetSeedData();
                WriteFile(seed);
                return seed;
            }

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
        }

        private void WriteFile(List<Book> books)
        {
            string json = JsonSerializer.Serialize(books,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public Book? GetById(int id)
        {
            return ReadFile().Find(b => b.Id == id);
        }

        public List<Book> GetAll(
            int pageNumber,
            int pageSize,
            bool? isAvailable = null,
            bool? hasFine = null,
            string? titleContains = null)
        {
            IEnumerable<Book> query = ReadFile();

            if (isAvailable.HasValue)
                query = query.Where(b => b.IsAvailable == isAvailable.Value);

            if (hasFine.HasValue)
                query = query.Where(b => hasFine.Value ? b.Fine > 0 : b.Fine == 0);

            if (!string.IsNullOrWhiteSpace(titleContains))
                query = query.Where(b =>
                    b.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));

            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public void Add(Book book)
        {
            var books = ReadFile();
            book.Id = books.Count == 0 ? 1 : books.Max(b => b.Id) + 1;
            books.Add(book);
            WriteFile(books);
        }

        public void Update(Book book)
        {
            var books = ReadFile();
            int index = books.FindIndex(b => b.Id == book.Id);
            if (index == -1) return;

            books[index] = book;
            WriteFile(books);
        }

        public void Delete(int id)
        {
            var books = ReadFile();
            books.RemoveAll(b => b.Id == id);
            WriteFile(books);
        }

        private static List<Book> GetSeedData() => new List<Book>
        {
            new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", IsAvailable = true },
            new Book { Id = 2, Title = "To Kill a Mockingbird", Author = "Harper Lee", IsAvailable = true },
            new Book { Id = 3, Title = "1984", Author = "George Orwell", IsAvailable = true },
            new Book { Id = 4, Title = "Pride and Prejudice", Author = "Jane Austen", IsAvailable = true },
            new Book { Id = 5, Title = "The Catcher in the Rye", Author = "J.D. Salinger", IsAvailable = true },
            new Book { Id = 6, Title = "Brave New World", Author = "Aldous Huxley", IsAvailable = true },
            new Book { Id = 7, Title = "Jane Eyre", Author = "Charlotte Bronte", IsAvailable = true },
            new Book { Id = 8, Title = "Wuthering Heights", Author = "Emily Bronte", IsAvailable = true }
        };
    }
}