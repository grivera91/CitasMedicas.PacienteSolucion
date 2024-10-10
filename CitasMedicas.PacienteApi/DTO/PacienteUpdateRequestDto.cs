namespace CitasMedicas.PacienteApi.DTO
{
    public class PacienteUpdateRequestDto
    {
        public string? NumeroHistoriaClinica { get; set; }
        public int? IdTipoSangre { get; set; }
        public string? Alergias { get; set; }
        public string? EnfermedadesPreexistentes { get; set; }
        public string? ContactoEmergencia { get; set; }
        public string? NumeroContactoEmergencia { get; set; }
        public string? Observaciones { get; set; }
        public string? UsuarioModificacion { get; set; }
    }
}
