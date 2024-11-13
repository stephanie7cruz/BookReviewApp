using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using BookReviewApp.Services;

namespace BookReviewApp.Services
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly string _jwtSecret;

        public AuthService(UserService userService)
        {
            _userService = userService;
            _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
        }

        // Registro de usuario (deberías asegurarte de hashear la contraseña antes de almacenarla)
        public async Task RegisterAsync(User user, string password)
        {
            // Hashear la contraseña antes de guardarla
            user.PasswordHash = HashPassword(password);

            await _userService.RegisterAsync(user);
        }

        // Login de usuario (renombrado a AuthenticateUserAsync)
        public async Task<string> AuthenticateUserAsync(string username, string password)
        {
            var user = await _userService.LoginAsync(username, password);
            if (user == null)
                return null; // Usuario no encontrado o contraseña incorrecta

            // Si las credenciales son válidas, generar un token JWT
            var token = GenerateJwtToken(user);

            return token;
        }

        // Generar un JWT para el usuario autenticado
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Username),
                new Claim("email", user.Email),
                new Claim("profile_picture", user.ProfilePictureUrl),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "BookReviewApp",
                audience: "BookReviewApp",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Validar el token JWT
        public ClaimsPrincipal ValidateJwtToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null)
                    return null;

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "BookReviewApp",
                    ValidAudience = "BookReviewApp",
                    IssuerSigningKey = key
                };

                var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        // Hashing de la contraseña (deberías usar algo más seguro como BCrypt en producción)
        private string HashPassword(string password)
        {
            // En producción, deberías usar un algoritmo de hashing seguro como BCrypt o Argon2
            return password; // Simulación simple
        }
    }
}
