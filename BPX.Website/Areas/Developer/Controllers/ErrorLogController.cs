using BPX.Service;
using BPX.Website.Controllers;
using BPX.Website.CustomCode.Authorize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace BPX.Website.Areas.Developer.Controllers
{
	[Area("Developer")]
	public class ErrorLogController : BaseController<ErrorLogController>
	{
		public ErrorLogController(ILogger<ErrorLogController> logger, ICoreService coreService) : base(logger, coreService)
		{
		}

		// GET or POST: /ErrorLog
		// GET or POST: /ErrorLog/Index
		////////////////////////////////////[Permit(Permits.Developer.ErrorLog.Index)]
		public IActionResult Index(string logDateString, string logAction)
		{
			DateTime logDate = DateTime.Now;

			if (logDateString != null && logDateString.Length > 9)
			{
				logDate = DateTime.Parse(logDateString);
			}

			if (logAction != null)
			{
				if (logAction.ToUpper().Equals("PREV"))
				{
					logDate = logDate.AddDays(-1);
				}
				else if (logAction.ToUpper().Equals("NEXT"))
				{
					logDate = logDate.AddDays(1);
				}
			}

			logDateString = logDate.ToString("yyyy-MM-dd");
			string pathErrorLogs = coreService.GetConfiguration().GetSection("AppSettings").GetSection("PathErrorLogs").Value;
			string logFile = pathErrorLogs + "\\BPXErrorlog-" + logDateString + ".log";
			string fileContent = "File not found for " + logDateString;

			if (System.IO.File.Exists(logFile))
			{
				fileContent = System.IO.File.ReadAllText(pathErrorLogs + "\\BPXErrorlog-" + logDateString + ".log", Encoding.UTF8);
				fileContent = fileContent.Replace("\n", "<br />");
			}

			ViewBag.logDateString = logDate.ToString("yyyy-MM-dd");
			ViewBag.fileContent = fileContent.Trim();

			return View();
		}

		// POST: /ErrorLog/DownloadLog
		[HttpPost]
		/////////////////////////////////////////[Permit(Permits.Developer.ErrorLog.DownloadLog)]
		public IActionResult DownloadLog(string logDateString, string logAction)
		{
			string pathErrorLogs = coreService.GetConfiguration().GetSection("AppSettings").GetSection("PathErrorLogs").Value;
			string logFile = pathErrorLogs + "\\BPXErrorlog-" + logDateString + ".log";
			string fileContent = "File not found for " + logDateString;

			if (logAction.ToUpper().Equals("DOWNLOAD") && System.IO.File.Exists(logFile))
			{
				fileContent = System.IO.File.ReadAllText(pathErrorLogs + "\\BPXErrorlog-" + logDateString + ".log", Encoding.UTF8);
				return File(Encoding.UTF8.GetBytes(fileContent), "text/plain", "BPXErrorlog-" + logDateString + ".log");
			}

			return View();
		}
	}
}