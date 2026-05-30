namespace ClinicManagementSystem.Models;

public class Patient
{
    public int PatientID { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string? Phone { get; set; }
    public DateOnly? BirthDate { get; set; }
}