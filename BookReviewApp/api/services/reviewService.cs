using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Data;

namespace BookReviewApp.Services
{
    public class ReviewService
    {
        private readonly string connectionString;

        public ReviewService()
        {
            connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        }

        public async Task<List<BookReview>> GetReviewsAsync(int bookId)
        {
            var reviews = new List<BookReview>();
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM Reviews WHERE BookId = @BookId", connection);
                command.Parameters.AddWithValue("@BookId", bookId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        reviews.Add(new BookReview
                        {
                            Id = reader.GetInt32("Id"),
                            BookId = reader.GetInt32("BookId"),
                            UserId = reader.GetInt32("UserId"),
                            Content = reader.GetString("Content"),
                            Rating = reader.GetInt32("Rating"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        });
                    }
                }
            }
            return reviews;
        }

        public async Task AddReviewAsync(BookReview bookReview)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "INSERT INTO Reviews (BookId, UserId, Content, Rating, CreatedAt) VALUES (@BookId, @UserId, @Content, @Rating, @CreatedAt)", connection);

                command.Parameters.AddWithValue("@BookId", bookReview.BookId);
                command.Parameters.AddWithValue("@UserId", bookReview.UserId);
                command.Parameters.AddWithValue("@Content", bookReview.Content);
                command.Parameters.AddWithValue("@Rating", bookReview.Rating);
                command.Parameters.AddWithValue("@CreatedAt", bookReview.CreatedAt);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateReviewAsync(BookReview bookReview)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "UPDATE Reviews SET Content = @Content, Rating = @Rating WHERE Id = @Id AND UserId = @UserId", connection);

                command.Parameters.AddWithValue("@Id", bookReview.Id);
                command.Parameters.AddWithValue("@UserId", bookReview.UserId);
                command.Parameters.AddWithValue("@Content", bookReview.Content);
                command.Parameters.AddWithValue("@Rating", bookReview.Rating);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteReviewAsync(int reviewId, int userId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "DELETE FROM Reviews WHERE Id = @Id AND UserId = @UserId", connection);

                command.Parameters.AddWithValue("@Id", reviewId);
                command.Parameters.AddWithValue("@UserId", userId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<BookReview>> GetReviewsByUserAsync(int userId)
        {
            var reviews = new List<BookReview>();
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM Reviews WHERE UserId = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        reviews.Add(new BookReview
                        {
                            Id = reader.GetInt32("Id"),
                            BookId = reader.GetInt32("BookId"),
                            UserId = reader.GetInt32("UserId"),
                            Content = reader.GetString("Content"),
                            Rating = reader.GetInt32("Rating"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        });
                    }
                }
            }
            return reviews;
        }
    }

    public class BookReview
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
