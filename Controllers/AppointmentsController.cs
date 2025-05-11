//appointment controller with dto 

using ClinicBooking.DTOs;
using ClinicBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Controllers
{
    /// <summary>
    /// Controller for Appointments
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="AppointmentsController"/>  class.
        /// </summary>
        /// <param name="context"></param>

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET all appointments
        /// <summary>
        /// Retrieves all appointments 
        /// </summary>
        /// <remarks>
        /// Sample Request:
        ///     Get /api/Appointments
        /// </remarks>
        /// <response code="200"> Appointments retrieved succesfull</response>
        /// <response code="404">No Appointments found</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IEnumerable<AppointmentReadDto>>> GetAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .ToListAsync();

            if (!appointments.Any())
                return NotFound(new { Message = "No appointments found." });

            var result = appointments.Select(a => new AppointmentReadDto
            {
                Id = a.Id,
                AppointmentDateTime = a.AppointmentDateTime,
                Category = a.Category,
                PatientName = a.Patient != null ? $"{a.Patient.FirstName} {a.Patient.LastName}" : null,
                DoctorName = a.Doctor != null ? $"{a.Doctor.FirstName} {a.Doctor.LastName}" : null,
                ClinicName = a.Clinic?.Name
            });

            return Ok(new { appointments = result });
        }

        // GET by ID
        /// <summary>
        /// Retrieves an appointment by its ID.
        /// </summary>
        /// <remarks>GET: /api/appointments/{id}</remarks>
        /// <param name="id">The ID of the appointment</param>
        /// <response code="200">Returns the appointment</response>
        /// <response code="404">If the appointment is not found</response>

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<AppointmentReadDto>> GetAppointment(int id)
        {
            var a = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (a == null)
                return NotFound(new { Message = "Appointment not found." });

            var dto = new AppointmentReadDto
            {
                Id = a.Id,
                AppointmentDateTime = a.AppointmentDateTime,
                Category = a.Category,
                PatientName = a.Patient != null ? $"{a.Patient.FirstName} {a.Patient.LastName}" : null,
                DoctorName = a.Doctor != null ? $"{a.Doctor.FirstName} {a.Doctor.LastName}" : null,
                ClinicName = a.Clinic?.Name
            };

            return Ok(new { appointment = dto });
        }

        // POST: create appointment
        /// <summary>
        /// Creates a new appointment.
        /// </summary>
        /// <remarks>POST: /api/appointments</remarks>
        /// <param name="dto">The appointment data</param>
        /// <response code="201">Returns the created appointment</response>
        /// <response code="400">If data is invalid or conflicts exist</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<AppointmentReadDto>> CreateAppointment([FromBody] AppointmentCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Category))
                return BadRequest(new { Message = "Category is required." });

            if (!await _context.Patients.AnyAsync(p => p.Id == dto.PatientId))
                return BadRequest(new { Message = "Patient ID does not exist." });

            if (!await _context.Doctors.AnyAsync(d => d.ID == dto.DoctorId))
                return BadRequest(new { Message = "Doctor ID does not exist." });

            if (!await _context.Clinics.AnyAsync(c => c.ID == dto.ClinicId))
                return BadRequest(new { Message = "Clinic ID does not exist." });

            bool timeClash = await _context.Appointments.AnyAsync(a =>
                a.PatientId == dto.PatientId &&
                a.AppointmentDateTime == dto.AppointmentDateTime);

            if (timeClash)
                return BadRequest(new { Message = "Patient already has an appointment at this time." });

            var appointment = new Appointment
            {
                AppointmentDateTime = dto.AppointmentDateTime,
                Category = dto.Category,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                ClinicId = dto.ClinicId
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            //  Re-fetch with navigation properties
            var created = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .FirstOrDefaultAsync(a => a.Id == appointment.Id);

            var resultDto = new AppointmentReadDto
            {
                Id = created!.Id,
                AppointmentDateTime = created.AppointmentDateTime,
                Category = created.Category,
                PatientName = created.Patient != null ? $"{created.Patient.FirstName} {created.Patient.LastName}" : null,
                DoctorName = created.Doctor != null ? $"{created.Doctor.FirstName} {created.Doctor.LastName}" : null,
                ClinicName = created.Clinic?.Name
            };

            return CreatedAtAction(nameof(GetAppointment), new { id = resultDto.Id }, resultDto);
        }



        // PUT: update
        /// <summary>
        /// Updates an existing appointment.
        /// </summary>
        /// <remarks>PUT: /api/appointments/{id}</remarks>
        /// <param name="id">The ID of the appointment to update</param>
        /// <param name="dto">The updated appointment data</param>
        /// <response code="200">If the update was successful</response>
        /// <response code="400">If the data is invalid</response>
        /// <response code="404">If the appointment is not found</response>


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentCreateDto dto)
        {
            var existing = await _context.Appointments.FindAsync(id);
            if (existing == null)
                return NotFound(new { Message = "Appointment not found." });

            // FK and validation
            if (!await _context.Patients.AnyAsync(p => p.Id == dto.PatientId))
                return BadRequest(new { Message = "Invalid patient." });

            if (!await _context.Doctors.AnyAsync(d => d.ID == dto.DoctorId))
                return BadRequest(new { Message = "Invalid doctor." });

            if (!await _context.Clinics.AnyAsync(c => c.ID == dto.ClinicId))
                return BadRequest(new { Message = "Invalid clinic." });

            if (string.IsNullOrWhiteSpace(dto.Category))
                return BadRequest(new { Message = "Category is required." });

            existing.AppointmentDateTime = dto.AppointmentDateTime;
            existing.Category = dto.Category;
            existing.PatientId = dto.PatientId;
            existing.DoctorId = dto.DoctorId;
            existing.ClinicId = dto.ClinicId;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Appointment updated successfully." });
        }

        // DELETE
        /// <summary>
        /// Deletes an appointment.
        /// </summary>
        /// <remarks>DELETE: /api/appointments/{id}</remarks>
        /// <param name="id">The ID of the appointment to delete</param>
        /// <response code="200">If the deletion was successful</response>
        /// <response code="404">If the appointment is not found</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound(new { Message = "Appointment not found." });

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Appointment deleted successfully." });
        }
    }
}
