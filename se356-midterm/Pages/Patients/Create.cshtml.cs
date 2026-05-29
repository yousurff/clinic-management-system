using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace se356_midterm.Pages.Patients
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public PatientInfo patientInfo = new PatientInfo();
        public string errorMessage = "";

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnPost()
        {
            patientInfo.FirstName = Request.Form["firstName"];
            patientInfo.LastName = Request.Form["lastName"];
            patientInfo.Phone = Request.Form["phone"];
            patientInfo.BirthDate = Request.Form["birthDate"];

            if (string.IsNullOrEmpty(patientInfo.FirstName) || string.IsNullOrEmpty(patientInfo.LastName))
            {
                errorMessage = "First and Last Name fields are required!";
                return;
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Patients (FirstName, LastName, Phone, BirthDate) VALUES (@fname, @lname, @phone, @bdate)";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@fname", patientInfo.FirstName);
                        command.Parameters.AddWithValue("@lname", patientInfo.LastName);
                        command.Parameters.AddWithValue("@phone", patientInfo.Phone);
                        command.Parameters.AddWithValue("@bdate", patientInfo.BirthDate);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Patients/Index");
        }
    }
}