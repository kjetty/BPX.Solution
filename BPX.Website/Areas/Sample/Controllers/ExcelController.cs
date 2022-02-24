using BPX.Service;
using BPX.Website.Controllers;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;

namespace BPX.Website.Areas.Sample.Controllers
{
	public class ExcelController : BaseController<ExcelController>
	{
		public ExcelController(ILogger<ExcelController> logger, ICoreService coreService) : base(logger, coreService)
        {

		}

		public IActionResult Index()
		{
			return View();
		}

        public IActionResult GenerateExcel()
        {
            // filepath is a string which contains the path where the new document has to be created
            //string filePath = @"c:\temp6\OpenXMLTest3.xlsx";

            //using (SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            //{
            //    // add a WorkbookPart to the document.
            //    WorkbookPart workbookPart = document.AddWorkbookPart();
            //    workbookPart.Workbook = new Workbook();

            //    // add a WorksheetPart to the WorkbookPart.
            //    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            //    worksheetPart.Worksheet = new Worksheet(new SheetData());

            //    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

            //    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Test Sheet" };

            //    sheets.Append(sheet);

            //    workbookPart.Workbook.Save();
            //}

            ////// Create a spreadsheet document by supplying the filepath.
            ////// By default, AutoSave = true, Editable = true, and Type = xlsx
            ////SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);

            ////// Add a WorkbookPart to the document.
            ////WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            ////workbookpart.Workbook = new Workbook();

            ////// Add a WorksheetPart to the WorkbookPart.
            ////WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            ////worksheetPart.Worksheet = new Worksheet(new SheetData());

            ////// Add Sheets to the Workbook.
            ////Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            ////// Append a new worksheet and associate it with the workbook.
            ////Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "mySheet" };
            ////sheets.Append(sheet);

            ////// Get the sheetData cell table.
            ////SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            ////// Add a row to the cell table.
            ////Row row;
            ////row = new Row() { RowIndex = 1 };
            ////sheetData.Append(row);

            ////// In the new row, find the column location to insert a cell in A1.
            ////Cell refCell = null;
            ////foreach (Cell cell in row.Elements<Cell>())
            ////{
            ////    if (string.Compare(cell.CellReference.Value, "A1", true) > 0)
            ////    {
            ////        refCell = cell;
            ////        break;
            ////    }
            ////}

            ////// Add the cell to the cell table at A1.
            ////Cell newCell = new Cell() { CellReference = "A1" };
            ////row.InsertBefore(newCell, refCell);

            ////// Set the cell value to be a numeric value of 100.
            ////newCell.CellValue = new CellValue("100");
            ////newCell.DataType = new EnumValue<CellValues>(CellValues.Number);

            ////// Close the document.
            ////spreadsheetDocument.Close();
            ///

            MemoryStream ms = new MemoryStream();

            //using (SpreadsheetDocument document = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(ms, SpreadsheetDocumentType.Workbook))
            {
                //WorkbookPart workbookPart = document.AddWorkbookPart();
                //workbookPart.Workbook = new Workbook();

                //WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                //worksheetPart.Worksheet = new Worksheet();

                //Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

                //Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Employees" };

                //sheets.Append(sheet);

                //SheetData sheetData = worksheetPart.Worksheet.AppendChild(new SheetData());

                //// Constructing header
                //Row row = new Row();
                //row.Append(
                //    ConstructCell("Id", CellValues.String),
                //    ConstructCell("Name", CellValues.String),
                //    ConstructCell("Birth Date", CellValues.String),
                //    ConstructCell("Salary", CellValues.String));

                //// Insert the header row to the Sheet Data
                //sheetData.AppendChild(row);

                //row = new Row();
                //row.Append(
                //    ConstructCell(123.ToString(), CellValues.Number),
                //    ConstructCell("fn001", CellValues.String),
                //    ConstructCell("10/10/2020", CellValues.String),
                //    ConstructCell("123.45", CellValues.Number));

                //sheetData.AppendChild(row);

                //worksheetPart.Worksheet.Save();

                ///////////////////////////////////////////////////////////////////

                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                //Sheets sheets = new Sheets();

                // sheet1
                WorksheetPart worksheetPart1 = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart1.Worksheet = new Worksheet();
                Sheet sheet1 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart1), SheetId = 1, Name = "Employees1" };

                sheets.Append(sheet1);

                SheetData sheetData1 = worksheetPart1.Worksheet.AppendChild(new SheetData());

                // Constructing header
                Row row1 = new Row();
                row1.Append(
                    ConstructCell("Id", CellValues.String),
                    ConstructCell("Name", CellValues.String),
                    ConstructCell("Birth Date", CellValues.String),
                    ConstructCell("Salary", CellValues.String));

                // Insert the header row to the Sheet Data
                sheetData1.AppendChild(row1);

                row1 = new Row();
                row1.Append(
                    ConstructCell(123.ToString(), CellValues.Number),
                    ConstructCell("fn001", CellValues.String),
                    ConstructCell("1/1/2020", CellValues.String),
                    ConstructCell("123.45", CellValues.Number));

                sheetData1.AppendChild(row1);

                worksheetPart1.Worksheet.Save();

                // sheet2
                WorksheetPart worksheetPart2 = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart2.Worksheet = new Worksheet();
                Sheet sheet2 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart2), SheetId = 2, Name = "Employees2" };

                sheets.Append(sheet2);

                SheetData sheetData2 = worksheetPart2.Worksheet.AppendChild(new SheetData());

                // Constructing header
                Row row2 = new Row();
                row2.Append(
                    ConstructCell("Id", CellValues.String),
                    ConstructCell("Name", CellValues.String),
                    ConstructCell("Birth Date", CellValues.String),
                    ConstructCell("Salary", CellValues.String));

                // Insert the header row to the Sheet Data
                sheetData2.AppendChild(row2);

                row2 = new Row();
                row2.Append(
                    ConstructCell(223.ToString(), CellValues.Number),
                    ConstructCell("fn002", CellValues.String),
                    ConstructCell("2/2/2020", CellValues.String),
                    ConstructCell("223.45", CellValues.Number));

                sheetData2.AppendChild(row2);

                worksheetPart2.Worksheet.Save();

                // sheet3
                WorksheetPart worksheetPart3 = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart3.Worksheet = new Worksheet();
                Sheet sheet3 = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart3), SheetId = 3, Name = "Employees3" };

                sheets.Append(sheet3);

                SheetData sheetData3 = worksheetPart3.Worksheet.AppendChild(new SheetData());

                // Constructing header
                Row row3 = new Row();
                row3.Append(
                    ConstructCell("Id", CellValues.String),
                    ConstructCell("Name", CellValues.String),
                    ConstructCell("Birth Date", CellValues.String),
                    ConstructCell("Salary", CellValues.String));

                // Insert the header row to the Sheet Data
                sheetData3.AppendChild(row3);

                row3 = new Row();
                row3.Append(
                    ConstructCell(333.ToString(), CellValues.Number),
                    ConstructCell("fn003", CellValues.String),
                    ConstructCell("3/3/2020", CellValues.String),
                    ConstructCell("333.45", CellValues.Number));

                sheetData3.AppendChild(row3);

                worksheetPart3.Worksheet.Save();
            }

            //return View();

            // rewind the memory stream
            ms.Seek(0, SeekOrigin.Begin);

            // return the file stream
            FileStreamResult rslt = new FileStreamResult(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            // back to the browser
            return rslt;
        }

        private Cell ConstructCell(string value, CellValues dataType)
        {
            return new Cell()
            {
                CellValue = new CellValue(value),
                DataType = new EnumValue<CellValues>(dataType)
            };
        }

    }
}
