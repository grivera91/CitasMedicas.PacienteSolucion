using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CitasMedicas.PacienteApi.Model
{
    public class Paciente
    {
        [Key]
        public int IdPaciente { get; set; }  // Clave primaria, con autoincremento

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }  // Clave foránea que referencia a la tabla Usuario

        [Required]
        [MaxLength(50)]
        public string NumeroHistoriaClinica { get; set; }  // Número único de la historia clínica

        [ForeignKey("TipoSangre")]
        public int? IdTipoSangre { get; set; }  // Clave foránea que referencia a la tabla TipoSangre

        public string Alergias { get; set; }  // Alergias conocidas del paciente (nullable)

        public string EnfermedadesPreexistentes { get; set; }  // Lista de enfermedades preexistentes del paciente (nullable)

        [MaxLength(100)]
        public string ContactoEmergencia { get; set; }  // Nombre del contacto de emergencia (nullable)

        [MaxLength(20)]
        public string NumeroContactoEmergencia { get; set; }  // Número de contacto de emergencia (nullable)

        public string Observaciones { get; set; }  // Observaciones adicionales (nullable)

        [Required]
        [MaxLength(20)]
        public string UsuarioCreacion { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;  // Fecha de creación del registro (fecha actual por defecto)

        [MaxLength(20)]
        public string? UsuarioModificacion { get; set; }  // Usuario que modificó este registro (nullable)

        public DateTime? FechaModificacion { get; set; }  // Fecha de la última modificación (nullable)
    }
}
