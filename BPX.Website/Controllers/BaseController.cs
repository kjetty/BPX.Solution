using BPX.Domain.CustomModels;
using BPX.Domain.ViewModels;
using BPX.Service;
using BPX.Utils;
using BPX.Website.CustomCode.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BPX.Website.Controllers
{
	public class BaseController<T> : Controller where T : class
	{
		// configuration
		protected readonly ICoreService coreService;
		protected readonly ILogger<T> logger;
		protected readonly IAccountService accountService;

		// bpx project variables
		protected int bpxPageSize;
		protected UserMeta currUserMeta;
		protected string currMenuString;
		
		public BaseController(ICoreService coreService, ILogger<T> logger, IAccountService accountService)
		{
			this.coreService = coreService;
			this.logger = logger;
			this.accountService = accountService;
			bpxPageSize = Convert.ToInt32(coreService.GetConfiguration().GetSection("AppSettings").GetSection("PageSize").Value);
			currUserMeta = new UserMeta();
		}

        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);

			if (ctx.HttpContext != null)
			{
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
							//var watch = new System.Diagnostics.Stopwatch();
							//watch.Start();
							
							string cacheKey = string.Empty;

							// get userMeta
							cacheKey = $"user:{userId}:meta";
							currUserMeta = coreService.GetCacheService().GetCache<UserMeta>(cacheKey);
							
							if (currUserMeta == null)
							{
								currUserMeta = accountService.GetUserMeta(userId);
								coreService.GetCacheService().SetCache(currUserMeta, cacheKey, coreService.GetCacheKeyService());
							}

							if (currUserMeta != null)
							{
								currUserMeta.LoginToken = loginToken;

								// get userRoleIds
								cacheKey = $"user:{userId}:roles";
								currUserMeta.UserRoleIds = coreService.GetCacheService().GetCache<List<int>>(cacheKey);

								if (currUserMeta.UserRoleIds == null)
								{
									currUserMeta.UserRoleIds = accountService.GetUserRoleIds(userId);
									coreService.GetCacheService().SetCache(currUserMeta.UserRoleIds, cacheKey, coreService.GetCacheKeyService());
								}

								// get userPermitIds
								cacheKey = $"user:{userId}:permits";
								currUserMeta.UserPermitIds = coreService.GetCacheService().GetCache<List<int>>(cacheKey);

								if (currUserMeta.UserPermitIds == null)
								{
									currUserMeta.UserPermitIds = accountService.GetUserPermitIds(currUserMeta.UserRoleIds);
									coreService.GetCacheService().SetCache(currUserMeta.UserPermitIds, cacheKey, coreService.GetCacheKeyService());
								}
																
								// get menuBar
								cacheKey = $"roles:{string.Join(string.Empty, currUserMeta.UserRoleIds)}:menu";
								currMenuString = coreService.GetCacheService().GetCache<string>(cacheKey);

								if (currMenuString == null)
								{
									currMenuString = accountService.GetUserMenuString(currUserMeta.UserRoleIds);
									coreService.GetCacheService().SetCache(currMenuString, cacheKey, coreService.GetCacheKeyService());
								}								

								//// populate ViewBag
								////ViewBag.currUserMeta = currUserMeta;
								ViewBag.currMenuString = currMenuString;
								ViewBag.currUserPermitIds = currUserMeta.UserPermitIds;

								////// Developer Override for Permits - BaseController (Part A) + PermitAttribute (PartB)
								////// OverrideOverrideOverride 
								////// use for testing only
								////// comment befor publishing
								////// START
								//if (currRequestMeta.host.Contains("localhost"))
								//{
								//	List<int> tempUserPermitIds = new List<int>();
								//	for (int i = 0; i < 10000; i++)
								//	{
								//		tempUserPermitIds.Add(i);
								//	}
								//	ViewBag.currUserPermitIds = tempUserPermitIds;
								//}
								////// END

								//watch.Stop();

								//double elapsedTime = (double)watch.ElapsedTicks / (double)Stopwatch.Frequency;
								//string executionTime = (elapsedTime * 1000000).ToString("F2") + " microseconds";
								//ShowAlert(AlertType.Info, $"Execution Time: {executionTime}");
							}
						}
					}
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