using CitasMedicas.PacienteApi.Data;
using CitasMedicas.PacienteApi.DTO;
using CitasMedicas.PacienteApi.Model;
using CitasMedicas.PacienteApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitasMedicas.PacienteApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly CorrelativoService _correlativoService;

        public PacienteController(ApplicationDbContext context, CorrelativoService correlativoService)
        {
            _context = context;
            _correlativoService = correlativoService;
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> CrearPaciente([FromBody] PacienteCreateRequestDto pacienteCreateRequestDto)
        {
            // Iniciar la transacción
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar si ya existe un paciente con el mismo IdUsuario
                bool pacienteExiste = await _context.Pacientes.AnyAsync(p => p.IdUsuario == pacienteCreateRequestDto.IdUsuario);
                if (pacienteExiste)
                {
                    return Conflict("Ya existe un paciente registrado con este usuario.");
                }

                string CodigoPaciente = await _correlativoService.ObtenerNuevoCorrelativoAsync("CP");
                string CodigoHistoriaClinica = await _correlativoService.ObtenerNuevoCorrelativoAsync("HC");

                // Crear el nuevo paciente basado en el DTO de request
                Paciente paciente = new Paciente
                {
                    IdUsuario = pacienteCreateRequestDto.IdUsuario,
                    CodigoPaciente = CodigoPaciente,
                    CodigoHistoriaClinica = CodigoHistoriaClinica,
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

                // Confirmar la transacción
                await transaction.CommitAsync();

                // Crear el DTO de respuesta
                PacienteCreateResponseDto pacienteCreateResponseDto = new PacienteCreateResponseDto
                {
                    IdPaciente = paciente.IdPaciente,
                    IdUsuario = paciente.IdUsuario,
                    CodigoPaciente = paciente.CodigoPaciente,
                    CodigoHistoriaClinica = paciente.CodigoHistoriaClinica,
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
                // Deshacer la transacción si algo falla
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error en el registro: {ex.Message}");
            }            
        }

        //[Authorize]
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
                CodigoPaciente = paciente.CodigoPaciente,
                CodigoHistoriaClinica = paciente.CodigoHistoriaClinica,
                IdTipoSangre = paciente.IdTipoSangre,
                Alergias = paciente.Alergias,
                EnfermedadesPreexistentes = paciente.EnfermedadesPreexistentes,
                ContactoEmergencia = paciente.ContactoEmergencia,
                NumeroContactoEmergencia = paciente.NumeroContactoEmergencia,
                Observaciones = paciente.Observaciones                
            };

            return Ok(response);
        }

        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PacienteCreateResponseDto>>> ListarPacientes()
        {
            try
            {
                var pacientes = await _context.Pacientes
                    .Select(p => new PacienteCreateResponseDto
                    {
                        IdPaciente = p.IdPaciente,
                        IdUsuario = p.IdUsuario,
                        CodigoHistoriaClinica = p.CodigoHistoriaClinica,
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

        //[Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> EditarPaciente(int id, [FromBody] PacienteUpdateRequestDto pacienteUpdateRequestDto)
        {
            // Iniciar la transacción
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Buscar el paciente en la base de datos
                Paciente? paciente = await _context.Pacientes.FindAsync(id);
                if (paciente == null)
                {
                    return NotFound(new { message = "Paciente no encontrado" });
                }

                // Actualizar solo los campos proporcionados en el request DTO
                paciente.CodigoHistoriaClinica = pacienteUpdateRequestDto.NumeroHistoriaClinica ?? paciente.CodigoHistoriaClinica;
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
                // Deshacer la transacción si algo falla
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error en el registro: {ex.Message}");
            }
        }
    }
}   