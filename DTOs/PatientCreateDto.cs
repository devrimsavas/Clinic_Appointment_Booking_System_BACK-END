using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.DTOs
{
    public class PatientCreateDto
    {
        /// <summary>
        /// The patient's first name.
        /// </summary>
        /// <example>John</example>
        [Required]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>The patient's last name.</summary>
        /// <example>Doe</example>
        [Required]
        public string LastName { get; set; } = string.Empty;
        /// <summary>The patient's email address.</summary>
        /// <example>john.doe@example.com</example>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        /// <summary>The patient's birth date.</summary>
        /// <example>1990-01-01</example>

        public DateTime? BirthDate { get; set; }
        /// <summary>The patient's gender.</summary>
        /// <example>Male</example>

        public string? Gender { get; set; }
    }
}
