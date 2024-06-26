using DinkToPdf.Contracts;
using DinkToPdf;
using VVA.ITS.WebApp.Interfaces;

namespace VVA.ITS.WebApp.Services
{
    public class PdfService : IPdfService
    {
        private readonly IConverter _converter;
        private readonly IWebHostEnvironment env;
        public PdfService(IConverter converter, IWebHostEnvironment env)
        {
            _converter = converter;
            this.env = env;
        }

        public byte[] GeneratePdf(string htmlContent)
        {
            var globalSettings = new GlobalSettings
            {
                PaperSize = PaperKind.A4Plus,
                Orientation = Orientation.Portrait,
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "UTF-8", LoadImages = true, UserStyleSheet = Path.Combine(this.env.ContentRootPath, "wwwroot", "css", "pdf.css") },

                //FooterSettings = { FontName = "Arial", FontSize = 12, Line = true, Center = "Biên bản xử phạt xe vi phạm tốc độ" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings },
               
            };

            return _converter.Convert(pdf);
        }
        public byte[] GeneratePdfWeigt(string htmlContent)
        {
            var globalSettings = new GlobalSettings
            {
                PaperSize = PaperKind.A4Plus,
                Orientation = Orientation.Portrait,
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "UTF-8", LoadImages = true, UserStyleSheet = Path.Combine(this.env.ContentRootPath, "wwwroot", "css", "pdf_weigh.css") },
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings },

            };

            return _converter.Convert(pdf);
        }
    }
}
