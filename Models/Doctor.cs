//doctor

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ClinicBooking.Models
{
    public class Doctor
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public int ClinicID { get; set; }
        public virtual Clinic? Clinic { get; set; }

        [Required]
        public int SpecialityID { get; set; }
        public virtual Speciality? Speciality { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
