using DinkToPdf;
using DinkToPdf.Contracts;
using System;
using System.IO;
using System.Threading.Tasks;

namespace billingApp.Services
{
    public class InvoiceService
    {
        private readonly IConverter _converter;

        public InvoiceService(IConverter converter)
        {
            _converter = converter;
        }

        public async Task<string> GenerateInvoicePdfAsync(string htmlContent)
        {
            var outputPath = Path.Combine("wwwroot", "invoices", $"invoice_{Guid.NewGuid()}.pdf");
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Out = outputPath
                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = htmlContent,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            _converter.Convert(doc);
            return outputPath;
        }
    }
}
