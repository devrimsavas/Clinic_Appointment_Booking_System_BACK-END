//speciality create dto 

namespace ClinicBooking.DTOs
{
    /// <summary>
    /// DTO used to create a new speciality.
    /// </summary>
    public class SpecialityCreateDto
    {
        /// <summary>
        /// The name of the speciality.
        /// </summary>
        /// <example>Dermatology</example>
        public string Name { get; set; } = string.Empty;
    }
}
