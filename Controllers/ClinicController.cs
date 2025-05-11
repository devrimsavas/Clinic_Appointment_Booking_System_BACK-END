//clinic controller with dto
using ClinicBooking.Models;
using ClinicBooking.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Controllers
{
    /// <summary>
    /// Controller for Clinics
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClinicsController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL clinics
        /// <summary>
        /// Retrieves all clinics.
        /// </summary>
        /// <remarks>GET: /api/clinics</remarks>
        /// <response code="200">Returns the list of clinics</response>
        /// <response code="404">If no clinics are found</response>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<ClinicReadDto>>> GetClinics()
        {
            var clinics = await _context.Clinics.ToListAsync();

            if (!clinics.Any())
                return NotFound(new { Message = "There is not any clinic yet." });

            var result = clinics.Select(c => new ClinicReadDto
            {
                ID = c.ID,
                Name = c.Name ?? "",
                Address = c.Address
            });

            return Ok(new { clinics = result });
        }

        // GET clinic by ID
        /// <summary>
        /// Retrieves a clinic by its ID.
        /// </summary>
        /// <param name="id">The ID of the clinic</param>
        /// <remarks>GET: /api/clinics/{id}</remarks>
        /// <response code="200">Returns the clinic</response>
        /// <response code="404">If the clinic is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ClinicReadDto>> GetClinic(int id)
        {
            var clinic = await _context.Clinics.FindAsync(id);

            if (clinic == null)
                return NotFound(new { Message = "Clinic not found." });

            var result = new ClinicReadDto
            {
                ID = clinic.ID,
                Name = clinic.Name ?? "",
                Address = clinic.Address
            };

            return Ok(new { clinic = result });
        }

        // POST a clinic
        /// <summary>
        /// Creates a new clinic.
        /// </summary>
        /// <param name="dto">Clinic data to create</param>
        /// <remarks>POST: /api/clinics</remarks>
        /// <response code="201">Clinic created successfully</response>
        /// <response code="400">Invalid input or duplicate name</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<ClinicReadDto>> CreateClinic([FromBody] ClinicCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { Message = "Clinic name is required." });

            bool nameExists = await _context.Clinics.AnyAsync(c => c.Name == dto.Name);
            if (nameExists)
                return BadRequest(new { Message = "A clinic with the same name already exists." });

            var clinic = new Clinic
            {
                Name = dto.Name,
                Address = dto.Address
            };

            _context.Clinics.Add(clinic);
            await _context.SaveChangesAsync();

            var result = new ClinicReadDto
            {
                ID = clinic.ID,
                Name = clinic.Name,
                Address = clinic.Address
            };

            return CreatedAtAction(nameof(GetClinic), new { id = clinic.ID }, result);
        }

        // PUT edit a clinic
        /// <summary>
        /// Updates an existing clinic.
        /// </summary>
        /// <param name="id">Clinic ID to update</param>
        /// <param name="dto">Updated clinic data</param>
        /// <remarks>PUT: /api/clinics/{id}</remarks>
        /// <response code="200">Clinic updated successfully</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">Clinic not found</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateClinic(int id, [FromBody] ClinicCreateDto dto)
        {
            var existing = await _context.Clinics.FindAsync(id);
            if (existing == null)
                return NotFound(new { Message = "Clinic not found." });

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { Message = "Clinic name is required." });

            existing.Name = dto.Name;
            existing.Address = dto.Address;

            await _context.SaveChangesAsync();

            var result = new ClinicReadDto
            {
                ID = existing.ID,
                Name = existing.Name,
                Address = existing.Address
            };

            return Ok(new { Message = "Clinic updated successfully.", clinic = result });
        }

        // DELETE clinic
        /// <summary>
        /// Deletes a clinic.
        /// </summary>
        /// <param name="id">The ID of the clinic to delete</param>
        /// <remarks>DELETE: /api/clinics/{id}</remarks>
        /// <response code="200">Clinic deleted successfully</response>
        /// <response code="400">Clinic has dependencies and cannot be deleted</response>
        /// <response code="404">Clinic not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteClinic(int id)
        {
            var clinic = await _context.Clinics
                .Include(c => c.Doctors)
                .Include(c => c.Appointments)
                .FirstOrDefaultAsync(c => c.ID == id);

            if (clinic == null)
                return NotFound(new { Message = "Clinic not found." });

            if (clinic.Doctors.Any() || clinic.Appointments.Any())
                return BadRequest(new { Message = "Cannot delete a clinic with doctors or appointments." });

            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Clinic deleted successfully." });
        }
    }
}
