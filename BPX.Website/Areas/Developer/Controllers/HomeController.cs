using BPX.Service;
using BPX.Website.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BPX.Website.Areas.Developer.Controllers
{
	[Area("Developer")]
	public class HomeController : BaseController<HomeController>
	{
		public HomeController(ILogger<HomeController> logger, ICoreService coreService) : base(logger, coreService)
		{
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}
