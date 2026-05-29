using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace se356_midterm.Pages.Departments
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public List<DepartmentInfo> listDepartments = new List<DepartmentInfo>();

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
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

    // Simple class to hold the data
    public class DepartmentInfo
    {
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }
}