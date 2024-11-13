using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BookReviewApp.Services;
using Newtonsoft.Json;

namespace BookReviewApp.Api.Auth
{
    public class AuthFunction
    {
        private readonly AuthService _authService;

        // Cambiar el constructor para inyectar UserService en AuthService
        public AuthFunction(UserService userService)
        {
            _authService = new AuthService(userService); // Crear una instancia de AuthService con el UserService
        }

        public async Task HandleRequest(HttpContext context)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            try
            {
                // Autenticar usuario
                if (context.Request.Method == "POST" && context.Request.Path == "/api/auth/login")
                {
                    using (var reader = new StreamReader(context.Request.Body))
                    {
                        var body = await reader.ReadToEndAsync();
                        var loginData = JsonConvert.DeserializeObject<LoginData>(body);

                        // Llamada a AuthenticateUserAsync con los datos de login
                        var user = await _authService.AuthenticateUserAsync(loginData.Username, loginData.Password);

                        if (user != null)
                        {
                            await response.WriteAsJsonAsync(user);
                        }
                        else
                        {
                            response.StatusCode = 401;
                            await response.WriteAsync("{\"error\":\"Invalid credentials\"}");
                        }
                    }
                }
                else
                {
                    response.StatusCode = 404;
                    await response.WriteAsync("{\"error\":\"Invalid route\"}");
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                await response.WriteAsync($"{{\"error\":\"{ex.Message}\"}}");
            }
        }
    }

    public class LoginData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
