using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace se356_midterm.Pages.Appointments
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<AppointmentInfo> listAppointments = new List<AppointmentInfo>();

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    // Fetch names by joining the Patients and Doctors tables using JOIN
                    string sql = @"
                        SELECT a.AppointmentID, p.FirstName, p.LastName, d.FirstName, d.LastName, a.AppointmentDateTime
                        FROM Appointments a
                        JOIN Patients p ON a.PatientID = p.PatientID
                        JOIN Doctors d ON a.DoctorID = d.DoctorID
                        ORDER BY a.AppointmentDateTime DESC";
                    
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AppointmentInfo appt = new AppointmentInfo();
                                appt.AppointmentID = reader.GetInt32(0).ToString();
                                appt.PatientName = reader.GetString(1) + " " + reader.GetString(2);
                                appt.DoctorName = "Dr. " + reader.GetString(3) + " " + reader.GetString(4);
                                appt.AppointmentDate = reader.GetDateTime(5).ToString("dd.MM.yyyy HH:mm");

                                listAppointments.Add(appt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

    public class AppointmentInfo
    {
        public string AppointmentID { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string AppointmentDate { get; set; }
    }
}