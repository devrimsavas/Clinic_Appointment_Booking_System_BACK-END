//search doctorcontroller with DTO 

using ClinicBooking.DTOs;
using ClinicBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Controllers
{
    /// <summary>
    /// Controller for searching doctors based on name
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SearchController(AppDbContext context)
        {
            _context = context;
        }

        //search doctor post 
        /// <summary>
        /// Searches for doctors by first name or last name
        /// </summary>
        /// <param name="request">Search criteria containing first or last name</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Search/doctors
        ///     {
        ///         "firstName": "John",
        ///         "lastName": "Doe"
        ///     }
        /// </remarks>
        /// <response code="200">Returns matching doctors with clinic and speciality info</response>
        /// <response code="400">No input provided for search</response>
        /// <response code="404">No matching doctors found</response>


        [HttpPost("doctors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> SearchDoctors([FromBody] SearchDoctorRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) && string.IsNullOrWhiteSpace(request.LastName))
            {
                return BadRequest(new { Message = "Please provide at least a first name or last name to search." });
            }

            var query = _context.Doctors
                .Include(d => d.Clinic)
                .Include(d => d.Speciality)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.FirstName))
            {
                query = query.Where(d => d.FirstName!.ToLower().Contains(request.FirstName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(request.LastName))
            {
                query = query.Where(d => d.LastName!.ToLower().Contains(request.LastName.ToLower()));
            }

            var results = await query
                .Select(d => new DoctorSearchResultDto
                {
                    FullName = d.FirstName + " " + d.LastName,
                    ClinicName = d.Clinic != null ? d.Clinic.Name : null,
                    SpecialityName = d.Speciality != null ? d.Speciality.Name : null
                })
                .ToListAsync();

            if (!results.Any())
            {
                return NotFound(new { Message = "No doctors found matching the search criteria." });
            }

            return Ok(new { results });
        }
    }
}
