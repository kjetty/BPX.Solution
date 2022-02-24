using BPX.Domain.ViewModels;
using BPX.Service;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
