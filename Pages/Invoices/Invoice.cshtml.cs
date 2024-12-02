using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.IO;
using System;

namespace BillingManagementSystem.Pages.Invoices
{
    public class CreateModel : PageModel
    {
        private readonly EmailService _emailService;
        private readonly PdfService _pdfService;

        public CreateModel(EmailService emailService, PdfService pdfService)
        {
            _emailService = emailService;
            _pdfService = pdfService;
        }

        public InvoiceInfo InvoiceInfo = new InvoiceInfo();
        public string ErrorMessage = "";
        public string SuccessMessage = "";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            InvoiceInfo.CustomerID = int.Parse(Request.Form["customerID"]);
            InvoiceInfo.Date = DateTime.Parse(Request.Form["date"]);
            InvoiceInfo.TotalAmount = decimal.Parse(Request.Form["totalAmount"]);

            if (InvoiceInfo.CustomerID == 0 ||
                InvoiceInfo.Date == DateTime.MinValue ||
                InvoiceInfo.TotalAmount == 0)
            {
                ErrorMessage = "All fields are required";
                return Page();
            }

            // Save invoice to DB
            try
            {
                string connectionString = "YourConnectionStringHere";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlQuery = "INSERT INTO Invoices (CustomerID, Date, TotalAmount) VALUES (@CustomerID, @Date, @TotalAmount);";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerID", InvoiceInfo.CustomerID);
                        command.Parameters.AddWithValue("@Date", InvoiceInfo.Date);
                        command.Parameters.AddWithValue("@TotalAmount", InvoiceInfo.TotalAmount);

                        command.ExecuteNonQuery();
                    }
                }

                // Generate PDF invoice content (use Razor or static HTML for this)
                string htmlContent = GenerateInvoiceHtml(InvoiceInfo);
                byte[] pdfContent = _pdfService.GeneratePdf(htmlContent);

                // Save the PDF to a file or send it as an attachment
                string filePath = Path.Combine("Invoices", $"Invoice_{InvoiceInfo.CustomerID}_{InvoiceInfo.Date.ToString("yyyyMMdd")}.pdf");
                System.IO.File.WriteAllBytes(filePath, pdfContent);

                // Send email with PDF attachment
                string customerEmail = "customer@example.com"; // Retrieve from DB based on CustomerID
                string subject = "Your Invoice from Billing Management System";
                string body = $"Dear Customer, your invoice of {InvoiceInfo.TotalAmount:C} is due on {InvoiceInfo.Date.ToShortDateString()}.";

                await _emailService.SendInvoiceEmailAsync(customerEmail, subject, body);

                SuccessMessage = "Invoice created, PDF generated, and sent successfully!";
                return RedirectToPage("/Invoices/Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }

        private string GenerateInvoiceHtml(InvoiceInfo invoice)
        {
            // Simple HTML template for the invoice
            return $@"
            <html>
                <body>
                    <h2>Invoice #{invoice.CustomerID}</h2>
                    <p>Date: {invoice.Date.ToString("yyyy-MM-dd")}</p>
                    <p>Total Amount: {invoice.TotalAmount:C}</p>
                    <p>Thank you for your business!</p>
                </body>
            </html>";
        }
    }

    public class InvoiceInfo
    {
        public int CustomerID { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
