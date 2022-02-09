using BPX.Service;
using BPX.Utils;
using BPX.Website.Controllers;
using BPX.Website.CustomCode.Authorize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BPX.Website.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ErrorLogsController : BaseController<ErrorLogsController>
	{
		public ErrorLogsController(ILogger<ErrorLogsController> logger, ICoreService coreService) : base(logger, coreService)
		{
		}

		// GET or POST: /ErrorLogs
		// GET or POST: /ErrorLogs/Index
		[Permit(Permits.Admin.ErrorLogs.Index)]
		public ActionResult Index(string logDateString, string logAction)
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

		// POST: /ErrorLogs/DownloadLog
		[HttpPost]
		[Permit(Permits.Admin.ErrorLogs.DownloadLog)]
		public ActionResult DownloadLog(string logDateString, string logAction)
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