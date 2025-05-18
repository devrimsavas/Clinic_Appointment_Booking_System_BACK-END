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
                DurationInMinutes = a.DurationInMinutes,
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
                DurationInMinutes = a.DurationInMinutes,
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

            if (dto.DurationInMinutes <= 0)
                return BadRequest(new { Message = "Duration must be greater than zero" });

            if (!await _context.Patients.AnyAsync(p => p.Id == dto.PatientId))
                return BadRequest(new { Message = "Patient ID does not exist." });

            if (!await _context.Doctors.AnyAsync(d => d.ID == dto.DoctorId))
                return BadRequest(new { Message = "Doctor ID does not exist." });

            if (!await _context.Clinics.AnyAsync(c => c.ID == dto.ClinicId))
                return BadRequest(new { Message = "Clinic ID does not exist." });

            var newStart = dto.AppointmentDateTime;
            var newEnd = newStart.AddMinutes(dto.DurationInMinutes);

            // Prevent overlapping appointments for same patient
            bool patientClash = await _context.Appointments.AnyAsync(a =>
                a.PatientId == dto.PatientId &&
                newStart < a.AppointmentDateTime.AddMinutes(a.DurationInMinutes) &&
                a.AppointmentDateTime < newEnd);

            if (patientClash)
                return BadRequest(new { Message = "Patient already has an overlapping appointment." });

            // Prevent overlapping appointments for same doctor
            bool doctorClash = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == dto.DoctorId &&
                newStart < a.AppointmentDateTime.AddMinutes(a.DurationInMinutes) &&
                a.AppointmentDateTime < newEnd);

            if (doctorClash)
                return BadRequest(new { Message = "Doctor is already booked at this time." });

            //Prevent appointment before actual time 
            if (dto.AppointmentDateTime < DateTime.Now)
                return BadRequest(new { Message = "You cannot book an appointment in the past." });

            //Restrict to working hours (08-18)
            var time = dto.AppointmentDateTime.TimeOfDay;
            if (time < TimeSpan.FromHours(8) || time > TimeSpan.FromHours(18))
                return BadRequest(new { Message = "Appointments must be booked between 08:00 and 18:00." });





            var appointment = new Appointment
            {
                AppointmentDateTime = dto.AppointmentDateTime,
                Category = dto.Category,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                ClinicId = dto.ClinicId,
                DurationInMinutes = dto.DurationInMinutes
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

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
                DurationInMinutes = created.DurationInMinutes,
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

            if (!await _context.Patients.AnyAsync(p => p.Id == dto.PatientId))
                return BadRequest(new { Message = "Invalid patient." });

            if (!await _context.Doctors.AnyAsync(d => d.ID == dto.DoctorId))
                return BadRequest(new { Message = "Invalid doctor." });

            if (!await _context.Clinics.AnyAsync(c => c.ID == dto.ClinicId))
                return BadRequest(new { Message = "Invalid clinic." });

            if (string.IsNullOrWhiteSpace(dto.Category))
                return BadRequest(new { Message = "Category is required." });

            var newStart = dto.AppointmentDateTime;
            var newEnd = newStart.AddMinutes(dto.DurationInMinutes);

            bool clash = await _context.Appointments.AnyAsync(a =>
                a.Id != id &&
                a.PatientId == dto.PatientId &&
                newStart < a.AppointmentDateTime.AddMinutes(a.DurationInMinutes) &&
                a.AppointmentDateTime < newEnd);

            if (clash)
                return BadRequest(new { Message = "Updated time conflicts with another appointment." });

            existing.AppointmentDateTime = dto.AppointmentDateTime;
            existing.Category = dto.Category;
            existing.PatientId = dto.PatientId;
            existing.DoctorId = dto.DoctorId;
            existing.ClinicId = dto.ClinicId;
            existing.DurationInMinutes = dto.DurationInMinutes;

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


        //add later SWAGGER do not forget 
        //SAMPLE 

        /*
        {
  "patient": {
    "firstName": "Jane",
    "lastName": "Doe",
    "email": "jane@example.com",
    "birthDate": "1992-08-15",
    "gender": "Female"
  },
  "appointment": {
    "appointmentDateTime": "2025-05-24T14:00:00",
    "category": "Checkup",
    "doctorId": 2,
    "clinicId": 1,
    "durationInMinutes": 30
  }
}

        */
        [HttpPost("with-patient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AppointmentReadDto>> CreateAppointmentWithPatient([FromBody] AppointmentWithPatientCreateDto dto)
        {
            var p = dto.Patient;
            var a = dto.Appointment;

            if (string.IsNullOrWhiteSpace(p.FirstName) || string.IsNullOrWhiteSpace(p.LastName) || string.IsNullOrWhiteSpace(p.Email))
                return BadRequest(new { Message = "Patient info is required." });

            if (string.IsNullOrWhiteSpace(a.Category))
                return BadRequest(new { Message = "Appointment category is required." });

            if (a.DurationInMinutes <= 0)
                return BadRequest(new { Message = "Duration must be greater than zero." });

            if (!await _context.Doctors.AnyAsync(d => d.ID == a.DoctorId))
                return BadRequest(new { Message = "Doctor not found." });

            if (!await _context.Clinics.AnyAsync(c => c.ID == a.ClinicId))
                return BadRequest(new { Message = "Clinic not found." });

            var patient = await _context.Patients.FirstOrDefaultAsync(x =>
                x.FirstName == p.FirstName &&
                x.LastName == p.LastName &&
                x.Email == p.Email);

            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    BirthDate = p.BirthDate,
                    Gender = p.Gender
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            var start = a.AppointmentDateTime;
            var end = start.AddMinutes(a.DurationInMinutes);

            // Check for patient conflict
            var patientClash = await _context.Appointments.AnyAsync(ap =>
                ap.PatientId == patient.Id &&
                start < ap.AppointmentDateTime.AddMinutes(ap.DurationInMinutes) &&
                ap.AppointmentDateTime < end);

            if (patientClash)
                return BadRequest(new { Message = "Patient already has a conflicting appointment." });

            // Check for doctor conflict
            var doctorClash = await _context.Appointments.AnyAsync(ap =>
                ap.DoctorId == a.DoctorId &&
                start < ap.AppointmentDateTime.AddMinutes(ap.DurationInMinutes) &&
                ap.AppointmentDateTime < end);

            if (doctorClash)
                return BadRequest(new { Message = "Doctor is already booked at this time." });

            //prevent booking in the past and out of working hours
            if (a.AppointmentDateTime < DateTime.Now)
                return BadRequest(new { Message = "You cannot book an appointment in the past." });

            var time = a.AppointmentDateTime.TimeOfDay;
            if (time < TimeSpan.FromHours(8) || time > TimeSpan.FromHours(18))
                return BadRequest(new { Message = "Appointments must be booked between 08:00 and 18:00." });



            var newAppointment = new Appointment
            {
                AppointmentDateTime = a.AppointmentDateTime,
                Category = a.Category,
                PatientId = patient.Id,
                DoctorId = a.DoctorId,
                ClinicId = a.ClinicId,
                DurationInMinutes = a.DurationInMinutes
            };

            _context.Appointments.Add(newAppointment);
            await _context.SaveChangesAsync();

            var created = await _context.Appointments
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .Include(x => x.Clinic)
                .FirstOrDefaultAsync(x => x.Id == newAppointment.Id);

            var result = new AppointmentReadDto
            {
                Id = created!.Id,
                AppointmentDateTime = created.AppointmentDateTime,
                Category = created.Category,
                DurationInMinutes = created.DurationInMinutes,
                PatientName = $"{created.Patient?.FirstName} {created.Patient?.LastName}",
                DoctorName = $"{created.Doctor?.FirstName} {created.Doctor?.LastName}",
                ClinicName = created.Clinic?.Name
            };

            return CreatedAtAction(nameof(GetAppointment), new { id = result.Id }, result);
        }

    }
}
