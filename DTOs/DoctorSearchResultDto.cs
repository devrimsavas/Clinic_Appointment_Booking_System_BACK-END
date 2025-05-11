//doctorsearchresult dto

namespace ClinicBooking.DTOs
{
    /// <summary>
    /// Represents the result of a doctor search.
    /// </summary>
    public class DoctorSearchResultDto
    {
        /// <summary>
        /// The full name of the doctor.
        /// </summary>
        /// <example>Emily Johnson</example>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// The name of the clinic where the doctor is assigned.
        /// </summary>
        /// <example>Sunrise Medical Center</example>
        public string? ClinicName { get; set; }

        /// <summary>
        /// The name of the doctor's speciality.
        /// </summary>
        /// <example>Pediatrics</example>
        public string? SpecialityName { get; set; }
    }
}

