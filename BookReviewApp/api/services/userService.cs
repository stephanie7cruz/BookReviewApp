using System;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace BookReviewApp.Services
{
    public class UserService
    {
        private readonly string connectionString;

        public UserService()
        {
            connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        }

        // Método para registrar un usuario
        public async Task RegisterAsync(User user)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@Username, @Email, @PasswordHash)", connection);

                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash); // Asegúrate de hashear la contraseña

                await command.ExecuteNonQueryAsync();
            }
        }

        // Método para iniciar sesión
        public async Task<User> LoginAsync(string username, string password)
        {
            User user = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM Users WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var storedPassword = reader.GetString("PasswordHash");
                        if (VerifyPassword(password, storedPassword))
                        {
                            user = new User
                            {
                                Id = reader.GetInt32("Id"),
                                Username = reader.GetString("Username"),
                                Email = reader.GetString("Email"),
                                ProfilePictureUrl = reader.GetString("ProfilePictureUrl")
                            };
                        }
                    }
                }
            }
            return user;
        }

        // Método para obtener el perfil de un usuario por su ID
        public async Task<User> GetUserProfileAsync(int userId)
        {
            User user = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM Users WHERE Id = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Email = reader.GetString("Email"),
                            ProfilePictureUrl = reader.GetString("ProfilePictureUrl")
                        };
                    }
                }
            }
            return user;
        }

        // Método para obtener un usuario por ID
        public async Task<User> GetUserByIdAsync(int userId)
        {
            User user = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand("SELECT * FROM Users WHERE Id = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new User
                        {
                            Id = reader.GetInt32("Id"),
                            Username = reader.GetString("Username"),
                            Email = reader.GetString("Email"),
                            ProfilePictureUrl = reader.GetString("ProfilePictureUrl")
                        };
                    }
                }
            }
            return user;
        }

        // Método para actualizar los datos de un usuario
        public async Task UpdateUserAsync(User updatedUser)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var command = new MySqlCommand(
                    "UPDATE Users SET Username = @Username, Email = @Email, ProfilePictureUrl = @ProfilePictureUrl WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Username", updatedUser.Username);
                command.Parameters.AddWithValue("@Email", updatedUser.Email);
                command.Parameters.AddWithValue("@ProfilePictureUrl", updatedUser.ProfilePictureUrl);
                command.Parameters.AddWithValue("@Id", updatedUser.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        // Método para verificar la contraseña (deberías usar un hashing seguro)
        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            // Aquí deberías usar un método de hashing seguro como BCrypt o Argon2
            return enteredPassword == storedPassword;
        }
    }

    // Modelo de Usuario
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
