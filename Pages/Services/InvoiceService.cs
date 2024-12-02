using DinkToPdf;
using DinkToPdf.Contracts;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AgriApp.Services
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
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Out = Path.Combine("wwwroot", "invoices", $"invoice_{Guid.NewGuid()}.pdf")
                },
                Objects = {
                    new ObjectSettings() { HtmlContent = htmlContent, WebSettings = { DefaultEncoding = "utf-8" } }
                }
            };

            _converter.Convert(doc);
            return doc.GlobalSettings.Out;
        }
    }
}
