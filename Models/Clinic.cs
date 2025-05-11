//clinic
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ClinicBooking.Models

{
    public class Clinic
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Address { get; set; }

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }



}
