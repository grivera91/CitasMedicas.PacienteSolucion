using CitasMedicas.PacienteApi.Model;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.PacienteApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Paciente> Pacientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Paciente>().ToTable("Paciente");
            base.OnModelCreating(modelBuilder);
        }
    }
}
