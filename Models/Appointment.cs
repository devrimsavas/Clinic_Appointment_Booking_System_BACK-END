//appointment model

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ClinicBooking.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        // Foreign keys
        [Required]
        public int PatientId { get; set; }
        public virtual Patient? Patient { get; set; }

        [Required]
        public int DoctorId { get; set; }
        public virtual Doctor? Doctor { get; set; }

        [Required]
        public int ClinicId { get; set; }
        public virtual Clinic? Clinic { get; set; }

        public int DurationInMinutes { get; set; }

    }



}
