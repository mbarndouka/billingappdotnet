using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;

namespace billingApp.Pages.Notifications
{
    public class CreateModel : PageModel
    {
        public NotificationInfo NotificationInfo = new NotificationInfo();
        public string ErrorMessage = "";
        public string SuccessMessage = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            NotificationInfo.CustomerID = int.Parse(Request.Form["customerID"]);
            NotificationInfo.Message = Request.Form["message"];
            NotificationInfo.Date = DateTime.Parse(Request.Form["date"]);

            if (NotificationInfo.CustomerID == 0 || string.IsNullOrEmpty(NotificationInfo.Message) || NotificationInfo.Date == DateTime.MinValue)
            {
                ErrorMessage = "All fields are required";
                return Page();
            }

            // Save the notification to the database
            try
            {
                string connectionString = "YourConnectionStringHere";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlQuery = "INSERT INTO Notifications (InvoiceId, Type, Message, SentAt) VALUES (@InvoiceId, @Type, @Message, @SentAt);";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@InvoiceId", NotificationInfo.CustomerID);
                        command.Parameters.AddWithValue("@Type", "Notification");
                        command.Parameters.AddWithValue("@Message", NotificationInfo.Message);
                        command.Parameters.AddWithValue("@SentAt", NotificationInfo.Date);

                        command.ExecuteNonQuery();
                    }
                }

                SuccessMessage = "Notification added successfully!";
                return RedirectToPage("/Notifications/Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }

    public class NotificationInfo
    {
        public int CustomerID { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
