using System.Text.Json;

namespace LibraryApp1.DataAccess
{
    public class BookRepository
    {
        private string filePath = "books.json";

        public List<Book> LoadBooks()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
            }
            else
            {
                var books = new List<Book>
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
                SaveBooks(books);
                return books;
            }
        }

        public void SaveBooks(List<Book> books)
        {
            string json = JsonSerializer.Serialize(books);
            File.WriteAllText(filePath, json);
        }
    }
}