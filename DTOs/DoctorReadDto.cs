//doctor read dto
namespace ClinicBooking.DTOs
{
    /// <summary>
    /// Data transfer object for reading doctor information.
    /// </summary>
    public class DoctorReadDto
    {
        /// <summary>
        /// The unique identifier of the doctor.
        /// </summary>
        /// <example>5</example>
        public int Id { get; set; }

        /// <summary>
        /// The doctor's first name.
        /// </summary>
        /// <example>Emily</example>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// The doctor's last name.
        /// </summary>
        /// <example>Johnson</example>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// The name of the clinic where the doctor is assigned.
        /// </summary>
        /// <example>Star Clinic</example>
        public string? ClinicName { get; set; }

        /// <summary>
        /// The name of the doctor's speciality.
        /// </summary>
        /// <example>Dermatology</example>
        public string? SpecialityName { get; set; }
    }
}

