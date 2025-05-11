//clinic create dto
using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.DTOs
{
    public class ClinicCreateDto
    {
        /// <summary>
        /// The name of the clinic.
        /// </summary>
        /// <example>Star Health Clinic</example>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The physical address of the clinic.
        /// </summary>
        /// <example>123 Main Street, Oslo</example>
        public string? Address { get; set; }
    }
}
