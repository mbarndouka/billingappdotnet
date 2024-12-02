using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace billingApp.Pages.billing
{
    public class PaymentModel : PageModel
    {
        public PaymentInfo paymentInfo = new PaymentInfo();
        public string ErrorMessage = "";
        public string SuccessMessage = "";

        private readonly IConfiguration _configuration;
        public PaymentModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            paymentInfo.Amount = Request.Form["amount"];
            paymentInfo.CustomerId = Request.Form["customerId"];

            if (string.IsNullOrEmpty(paymentInfo.Amount) || string.IsNullOrEmpty(paymentInfo.CustomerId))
            {
                ErrorMessage = "All fields are required";
                return;
            }

            try
            {
                string con = "Data Source=DESKTOP-T80LP34;Initial Catalog=billing;Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(con))
                {
                    connection.Open();
                    string sqlQuery = "INSERT INTO Payments (Amount, CustomerId) VALUES (@Amount, @CustomerId)";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Amount", paymentInfo.Amount);
                        command.Parameters.AddWithValue("@CustomerId", paymentInfo.CustomerId);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }
            SuccessMessage = "Payment processed successfully";
            Response.Redirect("/Payments/Index");
        }
    }

    public class PaymentInfo
    {
        public string Amount { get; set; }
        public string CustomerId { get; set; }
    }
}
