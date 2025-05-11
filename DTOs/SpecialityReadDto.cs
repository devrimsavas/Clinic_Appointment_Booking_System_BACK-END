//speciality read dto


namespace ClinicBooking.DTOs
{
    /// <summary>
    /// DTO for returning speciality data.
    /// </summary>
    public class SpecialityReadDto
    {
        /// <summary>
        /// The ID of the speciality.
        /// </summary>
        /// <example>1</example>
        public int ID { get; set; }

        /// <summary>
        /// The name of the speciality.
        /// </summary>
        /// <example>Cardiology</example>
        public string Name { get; set; } = string.Empty;
    }
}
