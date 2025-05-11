using ClinicBooking.DTOs;
using ClinicBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Controllers
{
    /// <summary>
    /// Controller for managing doctors
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoctorsController(AppDbContext context)
        {
            _context = context;
        }

        // GET all doctors
        /// <summary>
        /// Retrieves all doctors
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /api/Doctors
        /// </remarks>
        /// <response code="200">Doctors retrieved successfully</response>
        /// <response code="404">No doctors found</response>


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<DoctorReadDto>>> GetDoctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Speciality)
                .ToListAsync();

            if (!doctors.Any())
                return NotFound(new { Message = "No doctors found." });

            var result = doctors.Select(d => new DoctorReadDto
            {
                Id = d.ID,
                FirstName = d.FirstName ?? "",
                LastName = d.LastName ?? "",
                ClinicName = d.Clinic?.Name,
                SpecialityName = d.Speciality?.Name
            });

            return Ok(new { doctors = result });
        }

        // GET doctor by ID
        /// <summary>
        /// Retrieves a doctor by ID
        /// </summary>
        /// <param name="id">Doctor ID</param>
        /// <response code="200">Doctor found</response>
        /// <response code="404">Doctor not found</response>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<DoctorReadDto>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Speciality)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (doctor == null)
                return NotFound(new { Message = "Doctor not found." });

            var dto = new DoctorReadDto
            {
                Id = doctor.ID,
                FirstName = doctor.FirstName ?? "",
                LastName = doctor.LastName ?? "",
                ClinicName = doctor.Clinic?.Name,
                SpecialityName = doctor.Speciality?.Name
            };

            return Ok(new { doctor = dto });
        }

        // POST doctor
        /// <summary>
        /// Creates a new doctor
        /// </summary>
        /// <param name="dto">Doctor details</param>
        /// <response code="201">Doctor created successfully</response>
        /// <response code="400">Invalid input or doctor already exists</response>
        /// <response code="500">Doctor saved but retrieval failed</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<DoctorReadDto>> CreateDoctor([FromBody] DoctorCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                return BadRequest(new { Message = "First name and last name are required." });

            if (!await _context.Clinics.AnyAsync(c => c.ID == dto.ClinicID))
                return BadRequest(new { Message = "Clinic ID does not exist." });

            if (!await _context.Specialities.AnyAsync(s => s.ID == dto.SpecialityID))
                return BadRequest(new { Message = "Speciality ID does not exist." });

            bool duplicateExists = await _context.Doctors.AnyAsync(d =>
                d.FirstName == dto.FirstName &&
                d.LastName == dto.LastName &&
                d.ClinicID == dto.ClinicID);

            if (duplicateExists)
                return BadRequest(new { Message = "A doctor with the same name already exists in this clinic." });

            var doctor = new Doctor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                ClinicID = dto.ClinicID,
                SpecialityID = dto.SpecialityID
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            // Fetch for DTO response
            var created = await _context.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Speciality)
                .FirstOrDefaultAsync(d => d.ID == doctor.ID);

            //eliminate null check 
            if (created == null)
            {
                return StatusCode(500, new { Message = "Doctor was created but could not be retrieved." });

            }

            var resultDto = new DoctorReadDto
            {
                Id = created.ID,
                FirstName = created.FirstName!,
                LastName = created.LastName!,
                ClinicName = created.Clinic?.Name,
                SpecialityName = created.Speciality?.Name
            };

            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.ID }, resultDto);
        }


        // PUT doctor
        /// <summary>
        /// Updates an existing doctor
        /// </summary>
        /// <param name="id">Doctor ID</param>
        /// <param name="dto">Updated doctor details</param>
        /// <response code="200">Doctor updated successfully</response>
        /// <response code="400">Validation failed or foreign keys invalid</response>
        /// <response code="404">Doctor not found</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorCreateDto dto)
        {
            var existing = await _context.Doctors.FindAsync(id);
            if (existing == null)
                return NotFound(new { Message = "Doctor not found." });

            if (!await _context.Clinics.AnyAsync(c => c.ID == dto.ClinicID))
                return BadRequest(new { Message = "Clinic ID does not exist." });

            if (!await _context.Specialities.AnyAsync(s => s.ID == dto.SpecialityID))
                return BadRequest(new { Message = "Speciality ID does not exist." });

            existing.FirstName = dto.FirstName;
            existing.LastName = dto.LastName;
            existing.ClinicID = dto.ClinicID;
            existing.SpecialityID = dto.SpecialityID;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Doctor updated successfully." });
        }

        // DELETE doctor
        /// <summary>
        /// Deletes a doctor by ID
        /// </summary>
        /// <param name="id">Doctor ID</param>
        /// <response code="200">Doctor deleted successfully</response>
        /// <response code="400">Doctor has active appointments</response>
        /// <response code="404">Doctor not found</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.ID == id);

            if (doctor == null)
                return NotFound(new { Message = "Doctor not found." });

            if (doctor.Appointments.Any())
                return BadRequest(new { Message = "Cannot delete doctor with active appointments." });

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Doctor deleted successfully." });
        }
    }
}
