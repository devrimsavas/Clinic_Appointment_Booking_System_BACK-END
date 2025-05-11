//patient read dto 

namespace ClinicBooking.DTOs
{
    /// <summary>
    /// Represents a patient returned from the API.
    /// </summary>
    public class PatientReadDto
    {
        /// <summary>
        /// The unique ID of the patient.
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// The patient's first name.
        /// </summary>
        /// <example>Jane</example>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// The patient's last name.
        /// </summary>
        /// <example>Smith</example>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// The patient's email address.
        /// </summary>
        /// <example>jane.smith@example.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The patient's birth date.
        /// </summary>
        /// <example>1985-06-15</example>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// The patient's gender.
        /// </summary>
        /// <example>Female</example>
        public string? Gender { get; set; }
    }
}
