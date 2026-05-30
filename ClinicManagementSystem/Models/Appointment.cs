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

    // Hasta ilişkisi
    public int PatientID { get; set; }
    public Patient? Patient { get; set; }

    // Doktor ilişkisi
    public int DoctorID { get; set; }
    public Doctor? Doctor { get; set; }

    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? Notes { get; set; }
}