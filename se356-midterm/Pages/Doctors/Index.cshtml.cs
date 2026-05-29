using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace se356_midterm.Pages.Doctors
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<DoctorInfo> listDoctors = new List<DoctorInfo>();

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
                    string sql = @"SELECT d.DoctorID, d.FirstName, d.LastName, dep.DepartmentName 
                                   FROM Doctors d 
                                   LEFT JOIN Departments dep ON d.DepartmentID = dep.DepartmentID";
                    
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DoctorInfo doctor = new DoctorInfo();
                                doctor.DoctorID = reader.GetInt32(0).ToString();
                                doctor.FirstName = reader.GetString(1);
                                doctor.LastName = reader.GetString(2);
                                doctor.DepartmentName = reader.IsDBNull(3) ? "Not Specified" : reader.GetString(3);

                                listDoctors.Add(doctor);
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

    public class DoctorInfo
    {
        public string DoctorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }
}