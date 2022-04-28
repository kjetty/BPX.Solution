using BPX.Service;
using BPX.Website.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BPX.Website.Areas.Admin.Controllers
{
    [Area("Admin")]
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
