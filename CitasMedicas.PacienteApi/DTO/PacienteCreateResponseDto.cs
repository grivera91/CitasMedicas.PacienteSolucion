namespace CitasMedicas.PacienteApi.DTO
{
    public class PacienteCreateResponseDto
    {
        public int IdPaciente { get; set; }
        public int IdUsuario { get; set; }
        public string NumeroHistoriaClinica { get; set; }
        public int? IdTipoSangre { get; set; }
        public string Alergias { get; set; }
        public string EnfermedadesPreexistentes { get; set; }
        public string ContactoEmergencia { get; set; }
        public string NumeroContactoEmergencia { get; set; }
        public string Observaciones { get; set; }        
    }
}
