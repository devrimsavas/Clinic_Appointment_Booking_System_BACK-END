//appointment create dto
using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.DTOs
{
    public class AppointmentCreateDto
    {
        /// <summary>
        /// The date and time of the appointment.
        /// </summary>
        /// <example>2025-05-24T14:00:00</example>
        [Required]
        public DateTime AppointmentDateTime { get; set; }

        /// <summary>
        /// The category or reason for the appointment.
        /// </summary>
        /// <example>Dermatology</example>
        [Required]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the patient booking the appointment.
        /// </summary>
        /// <example>1</example>
        [Required]
        public int PatientId { get; set; }

        /// <summary>
        /// The ID of the doctor for the appointment.
        /// </summary>
        /// <example>2</example>
        [Required]
        public int DoctorId { get; set; }

        /// <summary>
        /// The ID of the clinic where the appointment will take place.
        /// </summary>
        /// <example>1</example>
        [Required]
        public int ClinicId { get; set; }
    }
}

