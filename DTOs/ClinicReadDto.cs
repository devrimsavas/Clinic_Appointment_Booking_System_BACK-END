//clinic read dto

namespace ClinicBooking.DTOs
{
    public class ClinicReadDto
    {
        /// <summary>
        /// The unique identifier of the clinic.
        /// </summary>
        /// <example>1</example>
        public int ID { get; set; }

        /// <summary>
        /// The name of the clinic.
        /// </summary>
        /// <example>Star Health Clinic</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The address of the clinic.
        /// </summary>
        /// <example>123 Main Street, Oslo</example>
        public string? Address { get; set; }
    }
}
