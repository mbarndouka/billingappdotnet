using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Collections.Generic;

namespace billingApp.Pages.Invoices
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

        [BindProperty]
        public InvoiceInfo InvoiceInfo { get; set; } = new InvoiceInfo();
        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";

        public void OnGet()
        {
            InvoiceInfo.InvoiceNumber = "122N"; // Generate a unique invoice number
            InvoiceInfo.IssueDate = DateTime.Now;
            InvoiceInfo.DueDate = DateTime.Now.AddDays(30);
            InvoiceInfo.Items = new List<InvoiceItem> { new InvoiceItem(), new InvoiceItem() };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
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
                    string sqlQuery = "INSERT INTO Invoices (InvoiceNumber, CustomerId, UserId, IssueDate, DueDate, TotalAmount, Status, TaxAmount) VALUES (@InvoiceNumber, @CustomerId, @UserId, @IssueDate, @DueDate, @TotalAmount, @Status, @TaxAmount);";
                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        command.Parameters.AddWithValue("@InvoiceNumber", InvoiceInfo.InvoiceNumber);
                        command.Parameters.AddWithValue("@CustomerId", InvoiceInfo.CustomerId);
                        command.Parameters.AddWithValue("@UserId", InvoiceInfo.UserId);
                        command.Parameters.AddWithValue("@IssueDate", InvoiceInfo.IssueDate);
                        command.Parameters.AddWithValue("@DueDate", InvoiceInfo.DueDate);
                        command.Parameters.AddWithValue("@TotalAmount", InvoiceInfo.TotalAmount);
                        command.Parameters.AddWithValue("@Status", InvoiceInfo.Status);
                        command.Parameters.AddWithValue("@TaxAmount", InvoiceInfo.TaxAmount);

                        command.ExecuteNonQuery();
                    }
                }

                // Generate PDF invoice content (use Razor or static HTML for this)
                string htmlContent = GenerateInvoiceHtml(InvoiceInfo);
                byte[] pdfContent = _pdfService.GeneratePdf(htmlContent);

                // Send email with PDF attachment
                string customerEmail = "customer@example.com"; // Retrieve from DB based on CustomerId
                string subject = "Your Invoice from Billing Management System";
                string body = $"Dear Customer, your invoice of {InvoiceInfo.TotalAmountWithVat:C} is due on {InvoiceInfo.DueDate.ToShortDateString()}.";

                await _emailService.SendInvoiceEmailAsync(customerEmail, subject, body, pdfContent);

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
                    <h2>Invoice #{invoice.InvoiceNumber}</h2>
                    <p>Date: {invoice.IssueDate.ToString("yyyy-MM-dd")}</p>
                    <p>Total Amount: {invoice.TotalAmountWithVat:C}</p>
                    <p>Thank you for your business!</p>
                </body>
            </html>";
        }
    }

    public class InvoiceInfo
    {
        public string InvoiceNumber { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string UserId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalAmountWithVat { get; set; }
        public string Status { get; set; }
        public decimal TaxAmount { get; set; }
    }

    public class InvoiceItem
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
