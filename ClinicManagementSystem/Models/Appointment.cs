using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public enum AppointmentStatus
{
    Pending,
    Approved,
    Completed,
    Cancelled
}

public class Appointment
{
    public int AppointmentID { get; set; }

    public int PatientID { get; set; }
    public Patient? Patient { get; set; }

    public int DoctorID { get; set; }
    public Doctor? Doctor { get; set; }

    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime AppointmentDate { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? Notes { get; set; }
}