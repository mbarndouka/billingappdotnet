using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;

public class PdfService
{
    private readonly IConverter _converter;

    public PdfService()
    {
        _converter = new BasicConverter(new PdfTools());
    }

    public byte[] GeneratePdf(string htmlContent)
    {
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = { PaperSize = PaperKind.A4, Orientation = Orientation.Portrait },
            Objects = { new ObjectSettings() { HtmlContent = htmlContent, WebSettings = { DefaultEncoding = "utf-8" } } }
        };

        return _converter.Convert(doc);
    }
}
