namespace ClinicManagementSystem.Models;

public class Doctor
{
    public int DoctorID { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? Phone { get; set; }

    // Bölüm ilişkisi (foreign key)
    public int DepartmentID { get; set; }
    public Department? Department { get; set; }

    public List<Appointment> Appointments { get; set; } = new();
}