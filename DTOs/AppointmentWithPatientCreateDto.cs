//DTOs/AppointmentWithPatientCreateDto.cs


using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.DTOs
{
    public class AppointmentWithPatientCreateDto
    {
        public PatientCreateDto Patient { get; set; } = default!;
        public AppointmentSubDto Appointment { get; set; } = default!;

    }

    public class AppointmentSubDto
    {
        public DateTime AppointmentDateTime { get; set; }
        public string Category { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public int DurationInMinutes { get; set; }
    }



}

