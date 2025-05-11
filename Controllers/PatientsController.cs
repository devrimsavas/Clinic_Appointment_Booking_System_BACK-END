using ClinicBooking.DTOs;
using ClinicBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Controllers
{
    /// <summary>
    /// Controller for managing patients
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }

        // GET all patients
        /// <summary>
        /// Retrieves all patients
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /api/Patients
        /// </remarks>
        /// <response code="200">Patients retrieved successfully</response>
        /// <response code="404">No patients found</response>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<PatientReadDto>>> GetPatients()
        {
            if (_context.Patients == null)
                return NotFound(new { Message = "No patients found." });

            var patients = await _context.Patients
                .Select(p => new PatientReadDto
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    BirthDate = p.BirthDate,
                    Gender = p.Gender
                })
                .ToListAsync();

            return Ok(new { patients });
        }

        // GET a patient by ID
        /// <summary>
        /// Retrieves a patient by ID
        /// </summary>
        /// <param name="id">Patient ID</param>
        /// <response code="200">Patient found</response>
        /// <response code="404">Patient not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<PatientReadDto>> GetPatient(int id)
        {
            if (_context.Patients == null)
                return NotFound(new { Message = "No patients available." });

            var p = await _context.Patients.FindAsync(id);

            if (p == null)
                return NotFound(new { Message = "Patient not found." });

            var patientDto = new PatientReadDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                BirthDate = p.BirthDate,
                Gender = p.Gender
            };

            return Ok(new { patient = patientDto });
        }

        // POST (create a patient)
        /// <summary>
        /// Creates a new patient
        /// </summary>
        /// <param name="patientDto">Patient data</param>
        /// <response code="201">Patient created successfully</response>
        /// <response code="400">Invalid data or duplicate email</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<PatientReadDto>> CreatePatient([FromBody] PatientCreateDto patientDto)
        {
            if (string.IsNullOrWhiteSpace(patientDto.FirstName) ||
                string.IsNullOrWhiteSpace(patientDto.LastName) ||
                string.IsNullOrWhiteSpace(patientDto.Email))
            {
                return BadRequest(new { Message = "Please provide valid patient data." });
            }

            bool emailExists = await _context.Patients.AnyAsync(p => p.Email == patientDto.Email);
            if (emailExists)
                return BadRequest(new { Message = "A patient with the same email already exists." });

            var newPatient = new Patient
            {
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                Email = patientDto.Email,
                BirthDate = patientDto.BirthDate,
                Gender = patientDto.Gender
            };

            _context.Patients.Add(newPatient);
            await _context.SaveChangesAsync();

            var resultDto = new PatientReadDto
            {
                Id = newPatient.Id,
                FirstName = newPatient.FirstName,
                LastName = newPatient.LastName,
                Email = newPatient.Email,
                BirthDate = newPatient.BirthDate,
                Gender = newPatient.Gender
            };

            return CreatedAtAction(nameof(GetPatient), new { id = resultDto.Id }, resultDto);
        }

        // PUT (update patient by ID)
        /// <summary>
        /// Updates an existing patient
        /// </summary>
        /// <param name="id">Patient ID</param>
        /// <param name="updated">Updated patient data</param>
        /// <response code="200">Patient updated successfully</response>
        /// <response code="400">Validation failed</response>
        /// <response code="404">Patient not found</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdatePatient(int id, [FromBody] PatientCreateDto updated)
        {
            var existing = await _context.Patients.FindAsync(id);
            if (existing == null)
                return NotFound(new { Message = "Patient not found." });

            if (string.IsNullOrWhiteSpace(updated.FirstName) ||
                string.IsNullOrWhiteSpace(updated.LastName) ||
                string.IsNullOrWhiteSpace(updated.Email))
            {
                return BadRequest(new { Message = "Please provide valid updated information." });
            }

            existing.FirstName = updated.FirstName;
            existing.LastName = updated.LastName;
            existing.Email = updated.Email;
            existing.BirthDate = updated.BirthDate;
            existing.Gender = updated.Gender;

            await _context.SaveChangesAsync();

            var resultDto = new PatientReadDto
            {
                Id = existing.Id,
                FirstName = existing.FirstName,
                LastName = existing.LastName,
                Email = existing.Email,
                BirthDate = existing.BirthDate,
                Gender = existing.Gender
            };

            return Ok(new { Message = "Patient updated successfully.", patient = resultDto });
        }

        // DELETE (delete patient by ID)
        /// <summary>
        /// Deletes a patient by ID
        /// </summary>
        /// <param name="id">Patient ID</param>
        /// <response code="200">Patient deleted successfully</response>
        /// <response code="400">Patient has active appointments</response>
        /// <response code="404">Patient not found</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
                return NotFound(new { Message = "Patient not found." });

            if (patient.Appointments.Any())
                return BadRequest(new { Message = "Cannot delete patient with active appointments." });

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Patient deleted successfully." });
        }
    }
}
