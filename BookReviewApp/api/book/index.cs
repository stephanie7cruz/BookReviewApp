using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BookReviewApp.Services; // Usamos BookItem de aquí para la lógica
using BookReviewApp.Models;   // Usamos BookItem de aquí si es necesario
using Newtonsoft.Json;

namespace BookReviewApp.Api.Book
{
    public class BookFunction
    {
        private readonly BookService _bookService;

        // Cambiar el constructor para inyectar BookService
        public BookFunction(BookService bookService)
        {
            _bookService = bookService;
        }

        public async Task HandleRequest(HttpContext context)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            try
            {
                // Obtener todos los libros
                if (context.Request.Method == "GET" && context.Request.Path == "/api/book")
                {
                    var books = await _bookService.GetBooksAsync(); // Usamos BookItem de Services
                    await response.WriteAsJsonAsync(books);
                }
                // Obtener un libro por ID
                else if (context.Request.Method == "GET" && context.Request.Path.StartsWithSegments("/api/book/{id}"))
                {
                    var bookId = int.Parse(context.Request.Path.Value.Split("/")[3]);
                    var book = await _bookService.GetBookByIdAsync(bookId); // Usamos BookItem de Services
                    if (book != null)
                    {
                        await response.WriteAsJsonAsync(book);
                    }
                    else
                    {
                        response.StatusCode = 404;
                        await response.WriteAsync("{\"error\":\"Book not found\"}");
                    }
                }
                // Agregar un nuevo libro
                else if (context.Request.Method == "POST" && context.Request.Path == "/api/book")
                {
                    using (var reader = new StreamReader(context.Request.Body))
                    {
                        var body = await reader.ReadToEndAsync();
                        var newBookItem = JsonConvert.DeserializeObject<BookItem>(body); // Usamos BookItem de Models

                        // Aseguramos que el nuevo libro sea válido y lo agregamos
                        if (newBookItem != null)
                        {
                            await _bookService.AddBookAsync(newBookItem); // Aquí pasa BookItem directamente
                            response.StatusCode = 201;
                            await response.WriteAsync("{\"message\":\"Book created successfully\"}");
                        }
                        else
                        {
                            response.StatusCode = 400;
                            await response.WriteAsync("{\"error\":\"Invalid book data\"}");
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
}
