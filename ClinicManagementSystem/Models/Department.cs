namespace ClinicManagementSystem.Models;

public class Department
{
    public int DepartmentID { get; set; }
    public string Name { get; set; } = "";

    public List<Doctor> Doctors { get; set; } = new();
}