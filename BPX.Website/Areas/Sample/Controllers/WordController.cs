using BPX.Service;
using BPX.Website.Controllers;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using DocRun = DocumentFormat.OpenXml.Wordprocessing.Run;
using ParagraphDoc = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using TextDoc = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace BPX.Website.Areas.Sample.Controllers
{
	public class WordController : BaseController<WordController>
    {
		public WordController(ILogger<WordController> logger, ICoreService coreService) : base(logger, coreService)
		{

		}

		public IActionResult Index()
		{
			return View();
		}

        public IActionResult GenerateWord()
        {
            // filepath is a string which contains the path where the new document has to be created
            string filePath = @"c:\temp8\OpenXMLTest2.docx";

            using (WordprocessingDocument doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();

                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(
                                        new Body(
                                            new ParagraphDoc(
                                                new DocRun(
                                                    new TextDoc("Hello World!!!!!")
                                                )
                                            )
                                        )
                                    );
            }

            return View();
        }

        public IActionResult GenerateDynamicWord()
        {
            MemoryStream ms = new MemoryStream();

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(
                    new Body(
                        new ParagraphDoc(
                            new DocRun(
                                new TextDoc("Hello World! " + DateTime.Now.ToString())
                            )
                        )
                    )
                );
            }

            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "DynamicWord.docx");

            //OR

            //// rewind the memory stream
            //ms.Seek(0, SeekOrigin.Begin);

            //// return the file stream
            //var rslt = new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.wordprocessingml.document") { FileDownloadName = "dyndoc.docx" };

            //// back to the browser
            //return rslt;
        }

        public IActionResult GenerateDynamicWord_working()
        {
            MemoryStream ms = new MemoryStream();

            using (ms = new MemoryStream())
            {
                using WordprocessingDocument wordDocument = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document);

                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(
                    new Body(
                        new ParagraphDoc(
                            new DocRun(
                                new TextDoc("Hello World! " + DateTime.Now.ToString())
                            )
                        )
                    )
                );

                ////mainPart.Document = new Document(
                ////	new Body(
                ////		new Paragraph(
                ////			new Run(
                ////				new Text("Hello world! 222 | " + DateTime.Now.ToString())))));

                ////// Create the document structure and add some text.
                //mainPart.Document = new Document();
                //Body docBody = new Body();

                //Paragraph p1 = new Paragraph();
                //Run r1 = new Run();
                //Text t1 = new Text("Lorem ipsum dolor sit amet, aaaaaa. | " + DateTime.Now.ToString());
                //r1.Append(t1);
                //p1.Append(r1);

                //docBody.Append(p1);

                //Paragraph p2 = new Paragraph(new Run(new Text("Lorem ipsum dolor sit amet, bbbbbb. | " + DateTime.Now.ToString())));
                //docBody.Append(p2);

                //// Run 2 - Bold
                //Paragraph p3 = new Paragraph();
                //Run r2 = new Run();
                //RunProperties rp2 = new RunProperties();
                //rp2.Bold = new Bold();
                //// Always add properties first
                //r2.Append(rp2);
                //Text t2 = new Text("commodo ") { Space = SpaceProcessingModeValues.Preserve };
                //r2.Append(t2);
                //p3.Append(r2);
                //docBody.Append(p3);

                //mainPart.Document.Append(docBody);
            }

            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "DynamicWord.docx");

            //// rewind the memory stream
            //ms.Seek(0, SeekOrigin.Begin);

            //// return the file stream
            //var rslt = new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            //// back to the browser
            //return rslt;
        }
    }
}
