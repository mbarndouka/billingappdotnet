using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace billingApp.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        public int TotalCustomers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OverduePayments { get; set; }

        public void OnGet()
        {
            try
            {
                string connectionString = "Data Source=DESKTOP-T80LP34;Initial Catalog=billing;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Get total customers
                    string sqlQuery = "SELECT COUNT(*) FROM Customers";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        TotalCustomers = (int)command.ExecuteScalar();
                    }

                    // Get total revenue
                    sqlQuery = "SELECT SUM(TotalAmount) FROM Invoices";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        TotalRevenue = (decimal)command.ExecuteScalar();
                    }

                    // Get overdue payments
                    sqlQuery = "SELECT COUNT(*) FROM Payments WHERE Date < GETDATE()";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        OverduePayments = (int)command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }
    }
}
