using BPX.Domain.CustomModels;
using BPX.Domain.ViewModels;
using BPX.Service;
using BPX.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
		protected readonly ILogger<T> logger;
		protected readonly ICoreService coreService;
		protected int bpxPageSize;
		protected UserMeta currUserMeta;
		protected string currMenuString;
		protected ICacheService cacheService;
		protected ICacheKeyService cacheKeyService;

		public BaseController(ILogger<T> logger, ICoreService coreService)
		{
			this.logger = logger;
			this.coreService = coreService;
			this.bpxPageSize = Convert.ToInt32(coreService.GetConfiguration().GetSection("AppSettings").GetSection("PageSize").Value);
			this.currUserMeta = new UserMeta();
			this.currMenuString = string.Empty;
			this.cacheService = coreService.GetCacheService();
			this.cacheKeyService = coreService.GetCacheKeyService();
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
						int userId = coreService.GetUserId(claimCurrLoginToken.Value);

						if (userId > 0)
						{
							//var watch = new System.Diagnostics.Stopwatch();
							//watch.Start();

							string cacheKey = string.Empty;

							// get userMeta
							cacheKey = $"user:{userId}:meta";
							currUserMeta = cacheService.GetCache<UserMeta>(cacheKey);
							
							if (currUserMeta == null)
							{
								currUserMeta = coreService.GetUserMeta(userId);
								cacheService.SetCache(currUserMeta, cacheKey, cacheKeyService);
							}

							if (currUserMeta != null)
							{
								currUserMeta.LoginToken = loginToken;

								// get userRoleIds
								cacheKey = $"user:{userId}:roles";
								currUserMeta.UserRoleIds = cacheService.GetCache<List<int>>(cacheKey);

								if (currUserMeta.UserRoleIds == null)
								{
									currUserMeta.UserRoleIds = coreService.GetUserRoleIds(userId);
									cacheService.SetCache(currUserMeta.UserRoleIds, cacheKey, cacheKeyService);
								}

								// get userPermitIds
								cacheKey = $"user:{userId}:permits";
								currUserMeta.UserPermitIds = cacheService.GetCache<List<int>>(cacheKey);

								if (currUserMeta.UserPermitIds == null)
								{
									currUserMeta.UserPermitIds = coreService.GetUserPermitIds(currUserMeta.UserRoleIds);
									cacheService.SetCache(currUserMeta.UserPermitIds, cacheKey, cacheKeyService);
								}
																
								// get menuBar
								cacheKey = $"roles:{string.Join(string.Empty, currUserMeta.UserRoleIds)}:menu";
								currMenuString = cacheService.GetCache<string>(cacheKey);

								if (currMenuString == null)
								{
									currMenuString = coreService.GetUserMenuString(currUserMeta.UserRoleIds);
									cacheService.SetCache(currMenuString, cacheKey, cacheKeyService);
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
    }
}