using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BookReviewApp.Services;
using BookReviewApp.Models;  
using Newtonsoft.Json;

namespace BookReviewApp.Api.Review
{
    public class ReviewFunction
    {
        private readonly ReviewService _reviewService;

        public ReviewFunction()
        {
            _reviewService = new ReviewService();
        }

        public async Task HandleRequest(HttpContext context)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            try
            {
                // Obtener todas las reseñas de un libro
                if (context.Request.Method == "GET" && context.Request.Path.StartsWithSegments("/api/review/{bookId}"))
                {
                    var bookId = int.Parse(context.Request.Path.Value.Split("/")[3]);
                    var reviews = await _reviewService.GetReviewsAsync(bookId);
                    await response.WriteAsJsonAsync(reviews);
                }
                // Agregar una reseña
                else if (context.Request.Method == "POST" && context.Request.Path == "/api/review")
                {
                    using (var reader = new StreamReader(context.Request.Body))
                    {
                        var body = await reader.ReadToEndAsync();
                        var newReview = JsonConvert.DeserializeObject<BookReview>(body);  // Usa la clase BookReview correctamente
                        await _reviewService.AddReviewAsync(newReview);
                        response.StatusCode = 201;
                        await response.WriteAsync("{\"message\":\"Review added successfully\"}");
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
