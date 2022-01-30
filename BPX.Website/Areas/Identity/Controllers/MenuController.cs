using BPX.Domain.ViewModels;
using BPX.Service;
using BPX.Website.Controllers;
using BPX.Website.CustomCode.Authorize;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPX.Website.Areas.Identity.Controllers
{
	[Area("Identity")]
	public class MenuController : BaseController<MenuController>
	{
		public MenuController(ILogger<MenuController> logger, ICoreService coreService) : base(logger, coreService)
		{

		}

		// GET: /Identity/Role
		public IActionResult Index()
		{
			return RedirectToAction("List");
		}

		// GET: /Identity/Role/List
		//[Permit(Permits.Identity.Menu.List)]
		[Permit(Permits.Identity.User.List)]
		public IActionResult List()
		{
			var menuList = coreService.GetMenuHierarchy();

			List<MenuMiniViewModel> model = new List<MenuMiniViewModel>();

			foreach (var itemMenu in menuList)
			{
				model.Add((MenuMiniViewModel)itemMenu);
			}

			return View(model);
		}


	}
}
