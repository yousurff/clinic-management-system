using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using se356_midterm.Pages.Doctors;
using se356_midterm.Pages.Patients;

namespace se356_midterm.Pages.Appointments
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string errorMessage = "";
        
        // Lists to display in the form
        public List<DoctorInfo> listDoctors = new List<DoctorInfo>();
        public List<PatientInfo> listPatients = new List<PatientInfo>();

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            // Populate patient and doctor lists when the page loads
            LoadDropdownData();
        }

        public void OnPost()
        {
            string patientID = Request.Form["patientID"];
            string doctorID = Request.Form["doctorID"];
            string appointmentDateStr = Request.Form["appointmentDate"];

            // Rule 1: Both patient and doctor must be selected
            if (string.IsNullOrEmpty(patientID) || string.IsNullOrEmpty(doctorID) || string.IsNullOrEmpty(appointmentDateStr))
            {
                errorMessage = "Please select the patient, doctor, and date completely!";
                LoadDropdownData();
                return;
            }

            DateTime appointmentDate;
            if (!DateTime.TryParse(appointmentDateStr, out appointmentDate))
            {
                errorMessage = "Invalid date format.";
                LoadDropdownData();
                return;
            }

            // Rule 2: Appointments cannot be booked for past dates
            if (appointmentDate < DateTime.Now)
            {
                errorMessage = "An appointment cannot be created for a past date or time!";
                LoadDropdownData();
                return;
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Rule 3: A doctor cannot have another appointment at the same date and time
                    string checkSql = "SELECT COUNT(*) FROM Appointments WHERE DoctorID=@docID AND AppointmentDateTime=@appDate";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkSql, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@docID", doctorID);
                        checkCmd.Parameters.AddWithValue("@appDate", appointmentDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            errorMessage = "This doctor already has an appointment at the selected date and time!";
                            LoadDropdownData();
                            return;
                        }
                    }

                    // If all rules pass, save the appointment to the database
                    string insertSql = "INSERT INTO Appointments (PatientID, DoctorID, AppointmentDateTime) VALUES (@patID, @docID, @appDate)";
                    using (MySqlCommand insertCmd = new MySqlCommand(insertSql, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@patID", patientID);
                        insertCmd.Parameters.AddWithValue("@docID", doctorID);
                        insertCmd.Parameters.AddWithValue("@appDate", appointmentDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                LoadDropdownData();
                return;
            }

            Response.Redirect("/Appointments/Index");
        }

        // Helper method to populate dropdowns
        private void LoadDropdownData()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Fetch patients
                    using (MySqlCommand cmd = new MySqlCommand("SELECT PatientID, FirstName, LastName FROM Patients", connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PatientInfo p = new PatientInfo();
                            p.PatientID = reader.GetInt32(0).ToString();
                            p.FirstName = reader.GetString(1);
                            p.LastName = reader.GetString(2);
                            listPatients.Add(p);
                        }
                    }

                    // Fetch doctors
                    using (MySqlCommand cmd = new MySqlCommand("SELECT DoctorID, FirstName, LastName FROM Doctors", connection))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DoctorInfo d = new DoctorInfo();
                            d.DoctorID = reader.GetInt32(0).ToString();
                            d.FirstName = reader.GetString(1);
                            d.LastName = reader.GetString(2);
                            listDoctors.Add(d);
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
}