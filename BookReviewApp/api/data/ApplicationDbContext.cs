// /BookReviewApp/Data/ApplicationDbContext.cs

using Microsoft.EntityFrameworkCore;
using BookReviewApp.Models; // Asegúrate de incluir el namespace de tus modelos

namespace BookReviewApp.Data
{
    // Clase que hereda de DbContext para manejar la conexión con la base de datos
    public class ApplicationDbContext : DbContext
    {
        // Constructor que recibe las opciones de configuración
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Definir los DbSets que representan las tablas de la base de datos
        public DbSet<AppUser> Users { get; set; }  // Agrega más DbSets si tienes otras tablas
    }
}
