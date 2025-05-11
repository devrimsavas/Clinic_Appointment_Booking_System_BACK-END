//doctor create dto 

using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.DTOs
{
    public class DoctorCreateDto
    {
        /// <summary>
        /// The doctor's first name.
        /// </summary>
        /// <example>Emily</example>
        [Required]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// The doctor's last name.
        /// </summary>
        /// <example>Johnson</example>
        [Required]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the clinic the doctor is assigned to.
        /// </summary>
        /// <example>2</example>
        [Required]
        public int ClinicID { get; set; }

        /// <summary>
        /// The ID of the doctor's speciality.
        /// </summary>
        /// <example>3</example>
        [Required]
        public int SpecialityID { get; set; }
    }
}
