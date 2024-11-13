using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BookReviewApp.Services;
using BookReviewApp.Models;  // Asegúrate de usar el espacio de nombres correcto
using Newtonsoft.Json;

namespace BookReviewApp.Api.User
{
    public class UserFunction
    {
        private readonly UserService _userService;

        public UserFunction()
        {
            _userService = new UserService();
        }

        public async Task HandleRequest(HttpContext context)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            try
            {
                // Obtener usuario por ID
                if (context.Request.Method == "GET" && context.Request.Path.StartsWithSegments("/api/user/{userId}"))
                {
                    var userId = int.Parse(context.Request.Path.Value.Split("/")[3]);
                    var user = await _userService.GetUserByIdAsync(userId);

                    if (user != null)
                    {
                        await response.WriteAsJsonAsync(user);
                    }
                    else
                    {
                        response.StatusCode = 404;
                        await response.WriteAsync("{\"error\":\"User not found\"}");
                    }
                }
                // Actualizar usuario
                else if (context.Request.Method == "PUT" && context.Request.Path.StartsWithSegments("/api/user/{userId}"))
                {
                    var userId = int.Parse(context.Request.Path.Value.Split("/")[3]);

                    using (var reader = new StreamReader(context.Request.Body))
                    {
                        var body = await reader.ReadToEndAsync();
                        var updatedUser = JsonConvert.DeserializeObject<AppUser>(body);  // Cambiado a AppUser
                        updatedUser.Id = userId;

                        await _userService.UpdateUserAsync(updatedUser);
                        await response.WriteAsync("{\"message\":\"User updated successfully\"}");
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
}
