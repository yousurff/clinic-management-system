using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace se356_midterm.Pages.Patients
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<PatientInfo> listPatients = new List<PatientInfo>();

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
                    string sql = "SELECT * FROM Patients";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PatientInfo patient = new PatientInfo();
                                patient.PatientID = reader.GetInt32(0).ToString();
                                patient.FirstName = reader.GetString(1);
                                patient.LastName = reader.GetString(2);
                                patient.Phone = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                patient.BirthDate = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd");

                                listPatients.Add(patient);
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

    public class PatientInfo
    {
        public string PatientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string BirthDate { get; set; }
    }
}