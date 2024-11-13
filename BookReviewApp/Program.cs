using Microsoft.EntityFrameworkCore;
using BookReviewApp.Models;
using BookReviewApp.Services;
using BookReviewApp.Data;  // Importa el espacio de nombres donde se encuentra ApplicationDbContext

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();

// Configuraci�n para Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuraci�n para la base de datos PlanetScale usando MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 30)))); // Ajusta seg�n la versi�n de MySQL que uses

// Inyecci�n de servicios
builder.Services.AddScoped<UserService>();  // Aseg�rate de registrar todos los servicios necesarios
builder.Services.AddScoped<AuthService>();  // Ejemplo para agregar el AuthService si lo tienes

// Construcci�n de la aplicaci�n
var app = builder.Build();

// Configuraci�n del pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ejecutar la aplicaci�n
app.Run();
