using Microsoft.EntityFrameworkCore;
using BookReviewApp.Models;
using BookReviewApp.Services;
using BookReviewApp.Data;  // Importa el espacio de nombres donde se encuentra ApplicationDbContext

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();

// Configuración para Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración para la base de datos PlanetScale usando MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 30)))); // Ajusta según la versión de MySQL que uses

// Inyección de servicios
builder.Services.AddScoped<UserService>();  // Asegúrate de registrar todos los servicios necesarios
builder.Services.AddScoped<AuthService>();  // Ejemplo para agregar el AuthService si lo tienes

// Construcción de la aplicación
var app = builder.Build();

// Configuración del pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ejecutar la aplicación
app.Run();
