using Resend;
using System.Threading.Tasks;

public class EmailService
{
    private readonly IResendClient _resendClient;

    public EmailService(IResendClient resendClient)
    {
        _resendClient = resendClient;
    }

    public async Task SendInvoiceEmailAsync(string toEmail, string subject, string body, byte[] pdfAttachment = null)
    {
        var emailMessage = new EmailMessage
        {
            To = toEmail,
            Subject = subject,
            Body = body
        };

        if (pdfAttachment != null)
        {
            emailMessage.Attachments = new[]
            {
                new EmailAttachment
                {
                    FileName = "Invoice.pdf",
                    Content = pdfAttachment,
                    ContentType = "application/pdf"
                }
            };
        }

        await _resendClient.SendEmailAsync(emailMessage);
    }
}
