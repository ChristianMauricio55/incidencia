using MiAplicacionMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MiAplicacionMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> //1.	Hereda de IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurar el esquema para PostgreSQL
            builder.HasDefaultSchema("public");

            // Asegurarse de que las tablas usen el formato correcto para PostgreSQL
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                // Reemplazar puntos por guiones bajos en los nombres de tabla
                var tableName = entity.GetTableName().Replace(".", "_");
                entity.SetTableName(tableName);
            }
        }

        public DbSet<Ticket> Tickets { get; set; }
    }
}
