using BPX.Service;
using BPX.Website.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BPX.Website.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ErrorLogsController :  BaseController<ErrorLogsController>
	{
		public ErrorLogsController(ILogger<ErrorLogsController> logger, ICoreService coreService) : base(logger, coreService)
		{
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
