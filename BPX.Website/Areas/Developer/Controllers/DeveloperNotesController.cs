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
    public class DeveloperNotesController : BaseController<DeveloperNotesController>
    {
        public DeveloperNotesController(ILogger<DeveloperNotesController> logger, ICoreService coreService) : base(logger, coreService)
        {
        }

        // GET or POST: /DeveloperNotes
        // GET or POST: /DeveloperNotes/Index
        //[Permit(Permits.Developer.DeveloperNotes.Index)]
        public IActionResult Index()
        {
            return View();
        }
    }
}