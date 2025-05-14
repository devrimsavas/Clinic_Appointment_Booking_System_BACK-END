//appointment read dto 

namespace ClinicBooking.DTOs
{
    public class AppointmentReadDto
    {
        /// <summary>
        /// The unique identifier of the appointment.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// The date and time of the appointment.
        /// </summary>
        /// <example>2025-05-24T14:00:00</example>
        public DateTime AppointmentDateTime { get; set; }

        /// <summary>
        /// The category or reason for the appointment.
        /// </summary>
        /// <example>Dermatology</example>
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// The full name of the patient attending the appointment.
        /// </summary>
        /// <example>Jane Doe</example>
        public string? PatientName { get; set; }

        /// <summary>
        /// The full name of the doctor for the appointment.
        /// </summary>
        /// <example>Dr. John Smith</example>
        public string? DoctorName { get; set; }

        /// <summary>
        /// The name of the clinic where the appointment is scheduled.
        /// </summary>
        /// <example>Star Clinic</example>
        public string? ClinicName { get; set; }

        public int DurationInMinutes { get; set; }
    }
}
