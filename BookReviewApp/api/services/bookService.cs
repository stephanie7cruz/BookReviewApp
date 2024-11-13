using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace BookReviewApp.Services
{
    public class BookService
    {
        private readonly string connectionString;

        public BookService()
        {
            connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        }

        public async Task<List<BookItem>> GetBooksAsync()
        {
            var books = new List<BookItem>();
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM Books", connection);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        books.Add(new BookItem
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Author = reader.GetString("Author"),
                            PublishedDate = reader.GetDateTime("PublishedDate"),
                            Category = reader.GetString("Category")
                        });
                    }
                }
            }
            return books;
        }

        public async Task<BookItem> GetBookByIdAsync(int id)
        {
            BookItem book = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM Books WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        book = new BookItem
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Author = reader.GetString("Author"),
                            PublishedDate = reader.GetDateTime("PublishedDate"),
                            Category = reader.GetString("Category")
                        };
                    }
                }
            }
            return book;
        }

        public async Task<List<BookItem>> GetBooksByCategoryAsync(string category)
        {
            var books = new List<BookItem>();
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM Books WHERE Category = @Category", connection);
                command.Parameters.AddWithValue("@Category", category);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        books.Add(new BookItem
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Author = reader.GetString("Author"),
                            PublishedDate = reader.GetDateTime("PublishedDate"),
                            Category = reader.GetString("Category")
                        });
                    }
                }
            }
            return books;
        }

        public async Task<List<BookItem>> SearchBooksAsync(string query)
        {
            var books = new List<BookItem>();
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "SELECT * FROM Books WHERE Title LIKE @Query OR Author LIKE @Query OR Category LIKE @Query", connection);
                command.Parameters.AddWithValue("@Query", "%" + query + "%");

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        books.Add(new BookItem
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Author = reader.GetString("Author"),
                            PublishedDate = reader.GetDateTime("PublishedDate"),
                            Category = reader.GetString("Category")
                        });
                    }
                }
            }
            return books;
        }

        // Método para agregar un nuevo libro
        public async Task AddBookAsync(BookItem bookItem)
        {
            if (bookItem == null) throw new ArgumentNullException(nameof(bookItem));

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("INSERT INTO Books (Title, Author, PublishedDate, Category) VALUES (@Title, @Author, @PublishedDate, @Category)", connection);
                command.Parameters.AddWithValue("@Title", bookItem.Title);
                command.Parameters.AddWithValue("@Author", bookItem.Author);
                command.Parameters.AddWithValue("@PublishedDate", bookItem.PublishedDate);
                command.Parameters.AddWithValue("@Category", bookItem.Category);

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    public class BookItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Category { get; set; }
    }
}
