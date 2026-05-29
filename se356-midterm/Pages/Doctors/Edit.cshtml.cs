using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using se356_midterm.Pages.Departments;

namespace se356_midterm.Pages.Doctors
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public DoctorInfo doctorInfo = new DoctorInfo();
        public List<DepartmentInfo> listDepartments = new List<DepartmentInfo>();
        public string errorMessage = "";

        public EditModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            string id = Request.Query["id"];
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    string sqlDoctor = "SELECT * FROM Doctors WHERE DoctorID=@id";
                    using (MySqlCommand command = new MySqlCommand(sqlDoctor, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                doctorInfo.DoctorID = reader.GetInt32(0).ToString();
                                doctorInfo.FirstName = reader.GetString(1);
                                doctorInfo.LastName = reader.GetString(2);
                                doctorInfo.DepartmentID = reader.GetInt32(3).ToString();
                            }
                        }
                    }

                    string sqlDepts = "SELECT * FROM Departments";
                    using (MySqlCommand command = new MySqlCommand(sqlDepts, connection))
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
            doctorInfo.DoctorID = Request.Form["id"];
            doctorInfo.FirstName = Request.Form["firstName"];
            doctorInfo.LastName = Request.Form["lastName"];
            doctorInfo.DepartmentID = Request.Form["departmentID"];

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Doctors SET FirstName=@fname, LastName=@lname, DepartmentID=@deptID WHERE DoctorID=@id";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@fname", doctorInfo.FirstName);
                        command.Parameters.AddWithValue("@lname", doctorInfo.LastName);
                        command.Parameters.AddWithValue("@deptID", doctorInfo.DepartmentID);
                        command.Parameters.AddWithValue("@id", doctorInfo.DoctorID);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Doctors/Index");
        }
    }
}