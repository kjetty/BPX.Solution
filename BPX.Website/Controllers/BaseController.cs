using BPX.Domain.ViewModels;
using BPX.Service;
using BPX.Utils;
using BPX.Website.CustomCode.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BPX.Website.Controllers
{
	public class BaseController<T> : Controller where T : BaseController<T>
	{
		// hosting environment
		private IHttpContextAccessor _httpContextAccessor;
		protected IHttpContextAccessor httpContextAccessor => _httpContextAccessor ??= HttpContext.RequestServices.GetService<IHttpContextAccessor>();

		// logger
		private ILogger<T> _logger;
		protected ILogger<T> logger => _logger ??= HttpContext.RequestServices.GetService<ILogger<T>>();

		// loginService
		private ILoginService _loginService;
		protected ILoginService loginService => _loginService ??= HttpContext.RequestServices.GetService<ILoginService>();

		// userService
		private IUserService _userService;
		protected IUserService userService => _userService ??= HttpContext.RequestServices.GetService<IUserService>();

		// userRoleService
		private IUserRoleService _userRoleService;
		protected IUserRoleService userRoleService => _userRoleService ??= HttpContext.RequestServices.GetService<IUserRoleService>();

		// rolePermitService
		private IRolePermitService _rolePermitService;
		protected IRolePermitService rolePermitService => _rolePermitService ??= HttpContext.RequestServices.GetService<IRolePermitService>();

		// menuService
		private IMenuService _menuService;
		protected IMenuService menuService => _menuService ??= HttpContext.RequestServices.GetService<IMenuService>();

		// menuRoleService
		private IMenuRoleService _menuRoleService;
		protected IMenuRoleService menuRoleService => _menuRoleService ??= HttpContext.RequestServices.GetService<IMenuRoleService>();

		// cache
		private IBPXCache _bpxCache;
		protected IBPXCache bpxCache => _bpxCache ??= HttpContext.RequestServices.GetService<IBPXCache>();

		// cacheKey
		private IMemoryCacheKeyService _memoryCacheKeyService;
		protected IMemoryCacheKeyService memoryCacheKeyService => _memoryCacheKeyService ??= HttpContext.RequestServices.GetService<IMemoryCacheKeyService>();

		// bpx project variables
		protected int bpxPageSize;
		protected int currUserId;
		protected string currLoginToken;
		protected List<int> currUserRoles;
		protected List<int> currUserPermits;
		protected string currHost;
		protected string currArea;
        protected string currController;
        protected string currAction;
		protected string currID;
		protected string developerViewBagOverride;
		protected string developerPermitAttributeOverride;
		protected string developerPasswordOverride;

		public BaseController()
		{
			developerViewBagOverride = "NO";
			developerPermitAttributeOverride = "NO";
			developerPasswordOverride = "NO";
			this.bpxPageSize = Convert.ToInt32(Startup.Configuration.GetSection("AppSettings").GetSection("PageSize").Value);
		}

        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);

			if (ctx.HttpContext != null)
			{
				currHost = ctx.HttpContext.Request.Host.ToString();				

				if (ctx.HttpContext.User != null)
			    {
					var claimCurrLoginToken = ctx.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "currLoginToken");

					if (claimCurrLoginToken != null)
					{
						currLoginToken = ctx.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "currLoginToken").Value;

						var login = loginService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.LoginToken.Equals(currLoginToken)).SingleOrDefault();

						if (login != null)
						{
							//var watch = new System.Diagnostics.Stopwatch();
							//watch.Start();

							string cacheKey = string.Empty;							
							currUserId = login.UserId;

							// listUserRoles
							cacheKey = $"user:{currUserId}:roles";
							currUserRoles = bpxCache.GetCache<List<int>>(cacheKey);

							if (currUserRoles == null)
							{
								currUserRoles = userRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.UserId == currUserId).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
								bpxCache.SetCache(currUserRoles, cacheKey, memoryCacheKeyService);
							}

							// listUserRolesPermits
							cacheKey = $"user:{currUserId}:permits";
							currUserPermits = bpxCache.GetCache<List<int>>(cacheKey);

							if (currUserPermits == null)
							{
								currUserPermits = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && currUserRoles.Contains(c.RoleId)).OrderBy(c => c.PermitID).Select(c => c.PermitID).Distinct().ToList();
								bpxCache.SetCache(currUserPermits, cacheKey, memoryCacheKeyService);
							}

							//List<int> listUserRoles = userRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.UserId == currUserId).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
							//List<int> listUserRolesPermits = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && listUserRoles.Contains(c.RoleId)).OrderBy(c => c.PermitID).Select(c => c.PermitID).Distinct().ToList();

							//watch.Stop();

							//double elapsedTime = (double)watch.ElapsedTicks / (double)Stopwatch.Frequency;
							//string executionTime = (elapsedTime * 1000000).ToString("F2") + " microseconds";

							////Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
							//ShowAlert(AlertType.Info, "Execution Time: " + executionTime);

							// populate ViewBag
							ViewBag.currHost = currHost;
							ViewBag.currUserId = currUserId;
							ViewBag.currUserRoles = currUserRoles;
							ViewBag.currUserPermits = currUserPermits;
						}
					}
				}
			}

			currArea = (ctx.RouteData.Values["area"] as string)?.ToLower();
			currController = (ctx.RouteData.Values["controller"] as string)?.ToLower();
			currAction = (ctx.RouteData.Values["action"] as string)?.ToLower();
			currID = (ctx.RouteData.Values["id"] as string)?.ToLower();

			ViewBag.menuString = GetMenuBar(currUserId, currUserRoles); 
			ViewBag.developerViewBagOverride = developerViewBagOverride;

			//// Developer Override 
			//// Development Override
			//// OverrideOverrideOverride :: todo
			//// comment out this code before publishing for PRODUCTION RELEASE
			//if (currHost.Contains("localhost"))
			//{
			//	string tempMessage = string.Empty; 
			//	developerViewBagOverride = Startup.Configuration.GetSection("DeveloperOverride").GetSection("ViewBagOverride").Value.ToString().Trim();
			//	developerPermitAttributeOverride = Startup.Configuration.GetSection("DeveloperOverride").GetSection("PermitAttributeOverride").Value.ToString().Trim();
			//	developerPasswordOverride = Startup.Configuration.GetSection("DeveloperOverride").GetSection("PasswordOverride").Value.ToString().Trim();

			//	if (developerViewBagOverride.Equals("YES-ForcedSet") || developerPermitAttributeOverride.Equals("YES-ForcedSet") || developerPasswordOverride.Equals("YES-ForcedSet"))
			//	{
			//		tempMessage += "CAUTION :: CAUTION :: CAUTION :: Developer Override is set.";					

			//		if (developerViewBagOverride.Equals("YES-ForcedSet"))
			//		{
			//			tempMessage += " :: In [BaseController].[OnActionExecuting()] for --developerViewBagOverride--.";
			//		}

			//		if (developerPermitAttributeOverride.Equals("YES-ForcedSet"))
			//		{
			//			tempMessage += " :: In [PermitAttribute].[OnAuthorization()] for --developerPermitAttributeOverride--.";
			//		}

			//		if (developerPermitAttributeOverride.Equals("YES-ForcedSet"))
			//		{
			//			tempMessage += " :: In [AccountController].[Login()] for --developerPasswordOverride--.";
			//		}

			//		ShowAlert(AlertType.Warning, tempMessage);
			//	}
			//}
		}

		protected void ShowAlert(AlertType alertType, string alertMessage)
        {
			string tempMessage = string.Empty;
			tempMessage += " " + (alertMessage ?? string.Empty);

            //// set alert
            //ViewBag.alertBox = new BootstrapAlertBox(alertType, tempMessage);

            // set alert
            TempData["alertBox"] = JsonConvert.SerializeObject(new BootstrapAlertBox(alertType, tempMessage));
        }

        protected string GetModelErrorMessage(ModelStateDictionary modelState)
        {
            return string.Join(" ", modelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
        }

        public static string GetGarneredErrorMessage(Exception ex)
        {
            Exception exception = ex;

            //get the message from the innermost exception
            while (exception.InnerException != null)
                exception = exception.InnerException;

            return exception.Message;
        }

		public string GetMenuBar(int userId, List<int> userRoleIds)
		{
			string cacheKey = $"user:{userId}:menu";
			string menuString = bpxCache.GetCache<string>(cacheKey);

			if (menuString == null)
			{
				var menuRoleList = menuRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals("A")).Select(c => c.MenuId).ToList();
				var menuList = menuService.GetRecordsByFilter(c => c.StatusFlag.Equals("A")).ToList();
				
				foreach (var itemMenu in menuList)
				{
					if (menuRoleList.Contains(itemMenu.MenuId))
					{
						menuString += $"<li class=\"nav-item\"><a class=\"nav-link text-dark\" href=\"{itemMenu.MenuURL}\">{itemMenu.MenuName}</a></li>";
					}
				}

				bpxCache.SetCache(menuString, cacheKey, memoryCacheKeyService);
			}

			return menuString;
		}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}