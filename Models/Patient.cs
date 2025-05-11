//patient


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ClinicBooking.Models

{

    public class Patient
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        public string? Gender { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
