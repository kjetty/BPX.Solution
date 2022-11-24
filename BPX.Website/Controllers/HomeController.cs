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
        public HomeController(ILogger<HomeController> logger, ICoreService coreService) : base(logger, coreService)
        {
        }

        public IActionResult Index()
        {
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
                logger.Log(LogLevel.Error, Environment.NewLine + "<<Global Error Handler>>" + Environment.NewLine + exceptionFeature.Path + Environment.NewLine + exceptionFeature.Error.Message + Environment.NewLine + exceptionFeature.Error.StackTrace);
            }
            else
            {
                logger.Log(LogLevel.Error, "An exception occurred. Error unkown");
            } 

            return View(new ErrorPageViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
