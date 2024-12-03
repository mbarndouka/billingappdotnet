using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace billingApp.Pages.Notifications
{
    public class IndexModel : PageModel
    {
        public List<NotificationInfo> Notifications { get; set; } = new List<NotificationInfo>();

        public void OnGet()
        {
            string connectionString = "YourConnectionStringHere";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlQuery = "SELECT InvoiceId AS CustomerID, Message, SentAt AS Date FROM Notifications";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Notifications.Add(new NotificationInfo
                            {
                                CustomerID = reader.GetInt32(0),
                                Message = reader.GetString(1),
                                Date = reader.GetDateTime(2)
                            });
                        }
                    }
                }
            }
        }
    }
}
