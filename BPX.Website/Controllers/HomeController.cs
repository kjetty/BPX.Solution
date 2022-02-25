using BPX.Domain.ViewModels;
using BPX.Service;
using BPX.Utils;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BPX.Website.Controllers
{
	public class HomeController : BaseController<HomeController>
	{

		private UserService _userService;

		public HomeController(ILogger<HomeController> logger, ICoreService coreService, IUserService userService) : base(logger, coreService)
		{
			_userService = (UserService)userService;
		}

		public IActionResult Index()
		{

			//int i = 0;
			//int j = 42 / i;

			//[80]	sBgT-X1hxxmh0-zljO-e17m-JFge


			List<string> listUUIds = new List<string>();

			for (int i=0; i < 100; i++)
            {
				string temp = Utility.GetUUID(20);
				string temp16 = temp.Insert(16, "-");
				string temp12 = temp16.Insert(12, "-");
				string temp8 = temp12.Insert(8, "-");
				string temp4 = temp8.Insert(4, "-");

				listUUIds.Add(temp4.Trim());
            }

			// abcd-efgh-ijkl-mnop-qrst

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
            IExceptionHandlerPathFeature exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			if (exceptionFeature != null)
			{
				logger.Log(LogLevel.Error, "In HomeController.Error() :: " + exceptionFeature.Path + " " + exceptionFeature.Error.Message + " " + exceptionFeature.Error.StackTrace);
			}
			else
			{
				logger.Log(LogLevel.Error, "An exception occurred. Error unkown");
			}

			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
