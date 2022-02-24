using BPX.Service;
using BPX.Website.Controllers;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection.Metadata;
using DocumentIText = iText.Layout.Document;
using ParagraphIText = iText.Layout.Element.Paragraph;

namespace BPX.Website.Areas.Sample.Controllers
{
    [Area("Sample")]
    public class PDFController : BaseController<PDFController>
	{
		public PDFController(ILogger<PDFController> logger, ICoreService coreService) : base(logger, coreService)
        {

		}

		public IActionResult Index()
		{
            GeneratePDFLocal();

            GeneratePDFDynamic();

            return View();
		}

        public IActionResult GeneratePDFLocal()
        {
            // must have write permissions to the path folder
            PdfWriter writer = new("C:\\temp\\demo.pdf");
            PdfDocument pdf = new(writer);
            DocumentIText document = new(pdf);
            ParagraphIText header = new ParagraphIText("HEADER")
               .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
               .SetFontSize(20);

            Paragraph subheader = new Paragraph("SUB HEADER")
               .SetTextAlignment(TextAlignment.CENTER)
               .SetFontSize(15);
            document.Add(subheader);

            document.Add(header);
            document.Close();

            return View();
        }

        public IActionResult GeneratePDFDynamic()
        {
            MemoryStream ms;

            using (ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                DocumentIText document = new iText.Layout.Document(pdf);

                document.Add(new ParagraphIText("Hello world! "));

                document.Close();
            }

            return File(ms.ToArray(), "application/pdf", "pdf_file_name.pdf");
        }
    }
}
