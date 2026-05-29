using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace se356_midterm.Pages.Departments
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public DepartmentInfo departmentInfo = new DepartmentInfo();
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
                    string sql = "SELECT * FROM Departments WHERE DepartmentID=@id";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                departmentInfo.DepartmentID = reader.GetInt32(0).ToString();
                                departmentInfo.DepartmentName = reader.GetString(1);
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
            departmentInfo.DepartmentID = Request.Form["id"];
            departmentInfo.DepartmentName = Request.Form["departmentName"];

            if (string.IsNullOrEmpty(departmentInfo.DepartmentName))
            {
                errorMessage = "Department name cannot be empty!";
                return;
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Departments SET DepartmentName=@name WHERE DepartmentID=@id";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", departmentInfo.DepartmentName);
                        command.Parameters.AddWithValue("@id", departmentInfo.DepartmentID);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Departments/Index");
        }
    }
}