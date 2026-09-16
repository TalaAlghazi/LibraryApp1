using Microsoft.Data.SqlClient;

namespace LibraryApp1.DataAccess
{
    public class AdoBookRepository : IBookRepository
    {
        private string _connectionString = "Server=.;Database=LibraryDb;Trusted_Connection=true;";

        public List<Book> GetAll(int pageNumber, int pageSize, bool? isAvailable = null, string? titleContains = null)
        {
            List<Book> books = new List<Book>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT * FROM Books 
                               WHERE (Title LIKE @title OR @title IS NULL)
                               AND (IsAvailable = @isAvailable OR @isAvailable IS NULL)
                               ORDER BY Id 
                               OFFSET @offset ROWS 
                               FETCH NEXT @pageSize ROWS ONLY";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", titleContains != null ? $"%{titleContains}%" : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@isAvailable", isAvailable.HasValue ? (object)isAvailable.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@offset", (pageNumber - 1) * pageSize);
                    cmd.Parameters.AddWithValue("@pageSize", pageSize);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books.Add(new Book
                            {
                                Id = (int)reader["Id"],
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                IsAvailable = (bool)reader["IsAvailable"]
                            });
                        }
                    }
                }
            }
            return books;
        }

        public Book GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Books WHERE Id = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Book
                            {
                                Id = (int)reader["Id"],
                                Title = reader["Title"].ToString(),
                                Author = reader["Author"].ToString(),
                                IsAvailable = (bool)reader["IsAvailable"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Add(Book book)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Books (Title, Author, IsAvailable) VALUES (@title, @author, @available)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", book.Title);
                    cmd.Parameters.AddWithValue("@author", book.Author);
                    cmd.Parameters.AddWithValue("@available", book.IsAvailable);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Book book)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Books SET Title = @title, Author = @author, IsAvailable = @available WHERE Id = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", book.Id);
                    cmd.Parameters.AddWithValue("@title", book.Title);
                    cmd.Parameters.AddWithValue("@author", book.Author);
                    cmd.Parameters.AddWithValue("@available", book.IsAvailable);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Books WHERE Id = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
