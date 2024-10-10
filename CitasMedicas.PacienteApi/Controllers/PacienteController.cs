using CitasMedicas.PacienteApi.Data;
using CitasMedicas.PacienteApi.DTO;
using CitasMedicas.PacienteApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.PacienteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PacienteController(ApplicationDbContext context)
        {
            _context = context;
        }
                
        [HttpPost]
        public async Task<IActionResult> CrearPaciente([FromBody] PacienteCreateRequestDto pacienteCreateRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Crear el nuevo paciente basado en el DTO de request
                Paciente paciente = new Paciente
                {
                    IdUsuario = pacienteCreateRequestDto.IdUsuario,
                    NumeroHistoriaClinica = pacienteCreateRequestDto.NumeroHistoriaClinica,
                    IdTipoSangre = pacienteCreateRequestDto.IdTipoSangre,
                    Alergias = pacienteCreateRequestDto.Alergias,
                    EnfermedadesPreexistentes = pacienteCreateRequestDto.EnfermedadesPreexistentes,
                    ContactoEmergencia = pacienteCreateRequestDto.ContactoEmergencia,
                    NumeroContactoEmergencia = pacienteCreateRequestDto.NumeroContactoEmergencia,
                    Observaciones = pacienteCreateRequestDto.Observaciones,
                    UsuarioCreacion = pacienteCreateRequestDto.UsuarioCreacion,
                    FechaCreacion = DateTime.Now
                };

                // Guardar el nuevo paciente en la base de datos
                _context.Pacientes.Add(paciente);
                await _context.SaveChangesAsync();

                // Crear el DTO de respuesta
                PacienteCreateResponseDto pacienteCreateResponseDto = new PacienteCreateResponseDto
                {
                    IdPaciente = paciente.IdPaciente,
                    IdUsuario = paciente.IdUsuario,
                    NumeroHistoriaClinica = paciente.NumeroHistoriaClinica,
                    IdTipoSangre = paciente.IdTipoSangre,
                    Alergias = paciente.Alergias,
                    EnfermedadesPreexistentes = paciente.EnfermedadesPreexistentes,
                    ContactoEmergencia = paciente.ContactoEmergencia,
                    NumeroContactoEmergencia = paciente.NumeroContactoEmergencia,
                    Observaciones = paciente.Observaciones
                };

                // Retornar CreatedAtAction con la URI del recurso creado
                return CreatedAtAction(nameof(ObtenerPacientePorId), new { id = paciente.IdPaciente }, pacienteCreateResponseDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }            
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPacientePorId(int id)
        {
            Paciente? paciente = await _context.Pacientes.FindAsync(id);

            if (paciente == null)
            {
                return NotFound();
            }

            // Crear el DTO de respuesta para el paciente
            PacienteCreateResponseDto response = new PacienteCreateResponseDto
            {
                IdPaciente = paciente.IdPaciente,
                IdUsuario = paciente.IdUsuario,
                IdTipoSangre = paciente.IdTipoSangre,
                Alergias = paciente.Alergias,
                EnfermedadesPreexistentes = paciente.EnfermedadesPreexistentes,
                ContactoEmergencia = paciente.ContactoEmergencia,
                NumeroContactoEmergencia = paciente.NumeroContactoEmergencia,
                Observaciones = paciente.Observaciones                
            };

            return Ok(response);
        }

        [HttpGet("listar")]
        public async Task<ActionResult<IEnumerable<PacienteCreateResponseDto>>> ListarPacientes()
        {
            try
            {
                var pacientes = await _context.Pacientes
                    .Select(p => new PacienteCreateResponseDto
                    {
                        IdPaciente = p.IdPaciente,
                        IdUsuario = p.IdUsuario,
                        NumeroHistoriaClinica = p.NumeroHistoriaClinica,
                        IdTipoSangre = p.IdTipoSangre,
                        Alergias = p.Alergias,
                        EnfermedadesPreexistentes = p.EnfermedadesPreexistentes,
                        ContactoEmergencia = p.ContactoEmergencia,
                        NumeroContactoEmergencia = p.NumeroContactoEmergencia,
                        Observaciones = p.Observaciones
                    })
                    .ToListAsync();

                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditarPaciente(int id, [FromBody] PacienteUpdateRequestDto pacienteUpdateRequestDto)
        {
            try
            {
                // Buscar el paciente en la base de datos
                Paciente? paciente = await _context.Pacientes.FindAsync(id);
                if (paciente == null)
                {
                    return NotFound(new { message = "Paciente no encontrado" });
                }

                // Actualizar solo los campos proporcionados en el request DTO
                paciente.NumeroHistoriaClinica = pacienteUpdateRequestDto.NumeroHistoriaClinica ?? paciente.NumeroHistoriaClinica;
                paciente.IdTipoSangre = pacienteUpdateRequestDto.IdTipoSangre ?? paciente.IdTipoSangre;
                paciente.Alergias = pacienteUpdateRequestDto.Alergias ?? paciente.Alergias;
                paciente.EnfermedadesPreexistentes = pacienteUpdateRequestDto.EnfermedadesPreexistentes ?? paciente.EnfermedadesPreexistentes;
                paciente.ContactoEmergencia = pacienteUpdateRequestDto.ContactoEmergencia ?? paciente.ContactoEmergencia;
                paciente.NumeroContactoEmergencia = pacienteUpdateRequestDto.NumeroContactoEmergencia ?? paciente.NumeroContactoEmergencia;
                paciente.Observaciones = pacienteUpdateRequestDto.Observaciones ?? paciente.Observaciones;
                paciente.UsuarioModificacion = pacienteUpdateRequestDto.UsuarioModificacion;
                paciente.FechaModificacion = DateTime.Now;

                // Guardar los cambios en la base de datos
                _context.Pacientes.Update(paciente);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Paciente actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}