//specialities controller with dto

using ClinicBooking.DTOs;
using ClinicBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Controllers
{
    /// <summary>
    /// Controller for managing specialities
    /// </summary>

    [ApiController]
    [Route("api/[controller]")]
    public class SpecialitiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SpecialitiesController(AppDbContext context)
        {
            _context = context;
        }

        // GET all specialities
        /// <summary>
        /// Retrieves all specialities
        /// </summary>
        /// <response code="200">Returns the list of specialities</response>
        /// <response code="404">No specialities found</response>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<SpecialityReadDto>>> GetSpecialities()
        {
            var specialities = await _context.Specialities.ToListAsync();

            if (!specialities.Any())
                return NotFound(new { Message = "No specialities found." });

            var result = specialities.Select(s => new SpecialityReadDto
            {
                ID = s.ID,
                Name = s.Name
            });

            return Ok(new { specialities = result });
        }

        // GET by ID
        /// <summary>
        /// Retrieves a specific speciality by ID
        /// </summary>
        /// <param name="id">Speciality ID</param>
        /// <response code="200">Returns the requested speciality</response>
        /// <response code="404">Speciality not found</response>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<SpecialityReadDto>> GetSpeciality(int id)
        {
            var speciality = await _context.Specialities.FindAsync(id);

            if (speciality == null)
                return NotFound(new { Message = "Speciality not found." });

            var dto = new SpecialityReadDto
            {
                ID = speciality.ID,
                Name = speciality.Name
            };

            return Ok(new { speciality = dto });
        }

        // POST
        /// <summary>
        /// Creates a new speciality
        /// </summary>
        /// <param name="dto">Speciality data</param>
        /// <response code="201">Speciality created successfully</response>
        /// <response code="400">Invalid or duplicate data</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<SpecialityReadDto>> CreateSpeciality([FromBody] SpecialityCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { Message = "Speciality name is required." });

            bool nameExists = await _context.Specialities.AnyAsync(s => s.Name == dto.Name);
            if (nameExists)
                return BadRequest(new { Message = "Speciality with this name already exists." });

            var speciality = new Speciality
            {
                Name = dto.Name
            };

            _context.Specialities.Add(speciality);
            await _context.SaveChangesAsync();

            var result = new SpecialityReadDto
            {
                ID = speciality.ID,
                Name = speciality.Name
            };

            return CreatedAtAction(nameof(GetSpeciality), new { id = speciality.ID }, result);
        }

        // PUT
        /// <summary>
        /// Updates an existing speciality
        /// </summary>
        /// <param name="id">Speciality ID</param>
        /// <param name="dto">Updated speciality data</param>
        /// <response code="200">Speciality updated successfully</response>
        /// <response code="400">Invalid data</response>
        /// <response code="404">Speciality not found</response>

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateSpeciality(int id, [FromBody] SpecialityCreateDto dto)
        {
            var existing = await _context.Specialities.FindAsync(id);

            if (existing == null)
                return NotFound(new { Message = "Speciality not found." });

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest(new { Message = "Speciality name is required." });

            existing.Name = dto.Name;
            await _context.SaveChangesAsync();

            var result = new SpecialityReadDto
            {
                ID = existing.ID,
                Name = existing.Name
            };

            return Ok(new { Message = "Speciality updated successfully.", speciality = result });
        }

        // DELETE
        /// <summary>
        /// Deletes a speciality
        /// </summary>
        /// <param name="id">Speciality ID</param>
        /// <response code="200">Speciality deleted successfully</response>
        /// <response code="400">Speciality is in use and cannot be deleted</response>
        /// <response code="404">Speciality not found</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteSpeciality(int id)
        {
            var speciality = await _context.Specialities
                .Include(s => s.Doctors)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (speciality == null)
                return NotFound(new { Message = "Speciality not found." });

            if (speciality.Doctors.Any())
                return BadRequest(new { Message = "Cannot delete a speciality that is assigned to doctors." });

            _context.Specialities.Remove(speciality);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Speciality deleted successfully." });
        }
    }
}
