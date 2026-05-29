using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace se356_midterm.Pages.Patients
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public PatientInfo patientInfo = new PatientInfo();
        public string errorMessage = "";

        public EditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            string id = Request.Query["id"];
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Patients WHERE PatientID=@id";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                patientInfo.PatientID = reader.GetInt32(0).ToString();
                                patientInfo.FirstName = reader.GetString(1);
                                patientInfo.LastName = reader.GetString(2);
                                patientInfo.Phone = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                patientInfo.BirthDate = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {
            patientInfo.PatientID = Request.Form["id"];
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
                    string sql = "UPDATE Patients SET FirstName=@fname, LastName=@lname, Phone=@phone, BirthDate=@bdate WHERE PatientID=@id";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@fname", patientInfo.FirstName);
                        command.Parameters.AddWithValue("@lname", patientInfo.LastName);
                        command.Parameters.AddWithValue("@phone", patientInfo.Phone);
                        command.Parameters.AddWithValue("@bdate", patientInfo.BirthDate);
                        command.Parameters.AddWithValue("@id", patientInfo.PatientID);
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