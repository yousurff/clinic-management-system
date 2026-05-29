using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using se356_midterm.Pages.Departments; 

namespace se356_midterm.Pages.Doctors
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public DoctorInfo doctorInfo = new DoctorInfo();
        public string errorMessage = "";
        
        public List<DepartmentInfo> listDepartments = new List<DepartmentInfo>();

        public CreateModel(IConfiguration configuration)
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
                    string sql = "SELECT * FROM Departments";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DepartmentInfo dept = new DepartmentInfo();
                                dept.DepartmentID = reader.GetInt32(0).ToString();
                                dept.DepartmentName = reader.GetString(1);
                                listDepartments.Add(dept);
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
            doctorInfo.FirstName = Request.Form["firstName"];
            doctorInfo.LastName = Request.Form["lastName"];
            doctorInfo.DepartmentID = Request.Form["departmentID"];

            if (string.IsNullOrEmpty(doctorInfo.FirstName) || string.IsNullOrEmpty(doctorInfo.LastName) || string.IsNullOrEmpty(doctorInfo.DepartmentID))
            {
                errorMessage = "All fields are mandatory!";
                OnGet(); 
                return;
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Doctors (FirstName, LastName, DepartmentID) VALUES (@fname, @lname, @deptID)";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@fname", doctorInfo.FirstName);
                        command.Parameters.AddWithValue("@lname", doctorInfo.LastName);
                        command.Parameters.AddWithValue("@deptID", doctorInfo.DepartmentID);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                OnGet();
                return;
            }

            Response.Redirect("/Doctors/Index");
        }
    }
}