using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace se356_midterm.Pages.Departments
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public CreateModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Variables to hold form data and display error messages
        public DepartmentInfo departmentInfo = new DepartmentInfo();
        public string errorMessage = "";

        public void OnGet()
        {
            // Runs when the page is first loaded. Empty for now.
        }

        public void OnPost()
        {
            // Get the data from the form
            departmentInfo.DepartmentName = Request.Form["departmentName"];

            // Prevent empty input
            if (string.IsNullOrEmpty(departmentInfo.DepartmentName))
            {
                errorMessage = "Department name cannot be empty!";
                return;
            }

            try
            {
                // Save to database
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Departments (DepartmentName) VALUES (@departmentName)";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@departmentName", departmentInfo.DepartmentName);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            // Redirect to the list page if the operation is successful
            Response.Redirect("/Departments/Index");
        }
    }
}