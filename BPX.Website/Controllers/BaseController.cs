using BPX.Domain.CustomModels;
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
		// logger
		private ILogger<T> _logger;
		protected ILogger<T> logger => _logger ??= HttpContext.RequestServices.GetService<ILogger<T>>();

		// cache
		private IBPXCache _bpxCache;
		protected IBPXCache bpxCache => _bpxCache ??= HttpContext.RequestServices.GetService<IBPXCache>();

		// accountService
		private IAccountService _accountService;
		protected IAccountService accountService => _accountService ??= HttpContext.RequestServices.GetService<IAccountService>();

		// cacheKey :: not required if using Redis
		private ICacheKeyService _CacheKeyService;
		protected ICacheKeyService CacheKeyService => _CacheKeyService ??= HttpContext.RequestServices.GetService<ICacheKeyService>();

		// bpx project variables
		protected int bpxPageSize;
		protected string currMenuString;
		protected UserMeta currUserMeta;
		protected RequestMeta currRequestMeta;
		protected DeveloperMeta currDeveloperMeta;

		public BaseController()
		{
			bpxPageSize = Convert.ToInt32(Startup.Configuration.GetSection("AppSettings").GetSection("PageSize").Value);
			currUserMeta = new UserMeta();
			currRequestMeta = new RequestMeta();
			currDeveloperMeta = new DeveloperMeta();
		}

        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);

			if (ctx.HttpContext != null)
			{
				// populate RequestMeta
				currRequestMeta.host = ctx.HttpContext.Request.Host.ToString().ToLower().Trim();
				currRequestMeta.area = (ctx.RouteData.Values["area"] as string)?.ToLower();
				currRequestMeta.controller = (ctx.RouteData.Values["controller"] as string)?.ToLower();
				currRequestMeta.action = (ctx.RouteData.Values["action"] as string)?.ToLower();
				currRequestMeta.id = (ctx.RouteData.Values["id"] as string)?.ToLower();

				//// Developer Override 
				//// OverrideOverrideOverride :: TODO
				//// comment code line 73-78 before publishing for PRODUCTION RELEASE
				// get DeveloperMeta
				if (currRequestMeta.host.Contains("localhost"))
				{
					currDeveloperMeta.ViewBagOverride = Startup.Configuration.GetSection("DeveloperMeta").GetSection("ViewBagOverride").Value.ToString().Trim();
					currDeveloperMeta.PermitAttributeOverride = Startup.Configuration.GetSection("DeveloperMeta").GetSection("PermitAttributeOverride").Value.ToString().Trim();
					currDeveloperMeta.PasswordOverride = Startup.Configuration.GetSection("DeveloperMeta").GetSection("PasswordOverride").Value.ToString().Trim();
				}

				// populate DeveloperMeta - ViewBag
				ViewBag.currDeveloperMeta = currDeveloperMeta;

				if (ctx.HttpContext.User != null)
			    {
					var claimCurrLoginToken = ctx.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "currLoginToken");

					if (claimCurrLoginToken != null)
					{
						// get current loginToken value
						string loginToken = claimCurrLoginToken.Value;

						// get user data from the loginToken
						// SECURITY - verify against the database for every request
						int userId = accountService.GetUserId(loginToken);

						if (userId > 0)
						{
							string cacheKey = string.Empty;

							// get userMeta
							cacheKey = $"user:{userId}:meta";
							currUserMeta = bpxCache.GetCache<UserMeta>(cacheKey);

							if (currUserMeta == null)
							{
								currUserMeta = accountService.GetUserMeta(userId);
								bpxCache.SetCache(currUserMeta, cacheKey, CacheKeyService);
							}

							if (currUserMeta != null)
							{
								string menuString = string.Empty;
								string userRoleIdsString = string.Join(string.Empty, currUserMeta.UserRoleIds);
								
								// get menuBar
								cacheKey = $"roles:{userRoleIdsString}:menu";
								currMenuString = bpxCache.GetCache<string>(cacheKey);

								if (currMenuString == null)
								{
									currMenuString = accountService.GetUserMenuString(currUserMeta.UserRoleIds);
									bpxCache.SetCache(currMenuString, cacheKey, CacheKeyService);
								}								

								// populate ViewBag
								ViewBag.currUserMeta = currUserMeta;
								ViewBag.currMenuString = currMenuString;
							}
						}
					}
				}
			}

			// show alert for developer override(s)
			if (currRequestMeta.host.Contains("localhost"))
			{
				if (currDeveloperMeta.ViewBagOverride.Equals("YES-ForcedSet") || currDeveloperMeta.PermitAttributeOverride.Equals("YES-ForcedSet") || currDeveloperMeta.PasswordOverride.Equals("YES-ForcedSet"))
				{
					string tempMessage = "CAUTION :: CAUTION :: CAUTION :: Developer Override is set.";

					if (currDeveloperMeta.ViewBagOverride.Equals("YES-ForcedSet"))
						tempMessage += " :: In [BaseController].[OnActionExecuting()] for --ViewBagOverride--.";
					
					if (currDeveloperMeta.PermitAttributeOverride.Equals("YES-ForcedSet"))
						tempMessage += " :: In [PermitAttribute].[OnAuthorization()] for --PermitAttributeOverride--.";

					if (currDeveloperMeta.PasswordOverride.Equals("YES-ForcedSet"))
						tempMessage += " :: In [AccountController].[Login()] for --PasswordOverride--.";

					ShowAlert(AlertType.Warning, tempMessage);
				}
			}
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

		protected string GetGarneredErrorMessage(Exception ex)
        {
            Exception exception = ex;

            //get the message from the innermost exception
            while (exception.InnerException != null)
                exception = exception.InnerException;

            return exception.Message;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}