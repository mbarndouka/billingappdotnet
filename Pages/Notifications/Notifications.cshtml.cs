using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace BillingManagementSystem.Pages.Notifications
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

            // Save the notification
            // (e.g., save to the database or notify the customer)

            SuccessMessage = "Notification added successfully!";
            return RedirectToPage("/Notifications/Index");
        }
    }

    public class NotificationInfo
    {
        public int CustomerID { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
