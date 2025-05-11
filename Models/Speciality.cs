//speciality 

using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.Models
{
    public class Speciality
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        //  navigation property to Doctors
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
