using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace billingApp.Pages.billing
{
    public class CustomersModel : PageModel
    {
        public CustomerInfo customerInfo = new CustomerInfo();
        public string ErrorMessage = "";
        public string SuccessMessage = "";

        private readonly IConfiguration _configuration;
        public CustomersModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void OnGet()
        {
        }

        public void OnPost() {
            customerInfo.Name = Request.Form["name"];
            customerInfo.Email = Request.Form["email"];
            customerInfo.Phone = Request.Form["phone"];
            customerInfo.Address = Request.Form["address"];
            

            if (string.IsNullOrEmpty(customerInfo.Name) ||
                string.IsNullOrEmpty(customerInfo.Email) || 
                string.IsNullOrEmpty(customerInfo.Phone) ||
                string.IsNullOrEmpty(customerInfo.Address)
                ) 
            { 
                ErrorMessage = "All fields are required";
                return; 
            }

            try 
            {
                String con = "Data Source=DESKTOP-8U3MOCH\\SQLEXPRESS;Initial Catalog=billing;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(con))
                { connection.Open(); 
                    string sqlQuery = "INSERT INTO Customers (Name, Email, Phone, Address) VALUES (@Name, @Email, @Phone, @Address);";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", customerInfo.Name);
                        command.Parameters.AddWithValue("@Email", customerInfo.Email); 
                        command.Parameters.AddWithValue("@Phone", customerInfo.Phone); 
                        command.Parameters.AddWithValue("@Address", customerInfo.Address); 
                        command.ExecuteNonQuery(); 
                    } 
                }
            } catch (Exception ex) {
                 ErrorMessage = ex.Message; 
                 return; 
            }
            customerInfo.Name = ""; 
            customerInfo.Email = ""; 
            customerInfo.Phone = ""; 
            customerInfo.Address = ""; 
            SuccessMessage = "New Customer added successfully";
            Response.Redirect("/Customers/Index");
        }
    }

    public class CustomerInfo
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
