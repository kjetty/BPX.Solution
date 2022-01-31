using BPX.Domain.CustomModels;
using BPX.Domain.DbModels;
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
		// protected vars 
		protected readonly ILogger<T> logger;
		protected readonly ICoreService coreService;
		protected int bpxPageSize;
		protected UserMeta currUserMeta;
		protected string currMenuString;
		// private vars
		private ICacheService cacheService;
		private ICacheKeyService cacheKeyService;

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
						// SECURITY SECURITY SECURITY :: always verify against the database on every request
						int userId = coreService.GetUserId(claimCurrLoginToken.Value);

						if (userId > 0)
						{
							//var watch = new System.Diagnostics.Stopwatch();
							//watch.Start();

							// get userMeta
							currUserMeta = GetUserMeta(userId);

							if (currUserMeta != null)
							{
								// apply loginToken
								currUserMeta.LoginToken = loginToken;

								// get and apply userRoleIds
								currUserMeta.UserRoleIds = GetUserRoleIds(userId);								

								// get and apply userPermitIds
								currUserMeta.UserPermitIds = GetUserPermitIds(userId, currUserMeta.UserRoleIds);								

								// get menuString
								currMenuString = GetMenuString(currUserMeta.UserRoleIds, currUserMeta.UserPermitIds);

								// populate ViewBag
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
								//ShowAlertBox(AlertType.Info, $"Execution Time: {executionTime}");
							}
						}
					}
				}
			}
		}

		private UserMeta GetUserMeta(int userId)
		{
			string cacheKey = $"user:{userId}:meta";
			UserMeta userMeta = cacheService.GetCache<UserMeta>(cacheKey);

			if (userMeta == null)
			{
				userMeta = coreService.GetUserMeta(userId);
				cacheService.SetCache(userMeta, cacheKey, cacheKeyService);
			}

			return userMeta;
		}

		private List<int> GetUserRoleIds(int userId)
		{
			string cacheKey = $"user:{userId}:roles";
			List<int> userRoleIds = cacheService.GetCache<List<int>>(cacheKey);

			if (userRoleIds == null)
			{
				userRoleIds = coreService.GetUserRoleIds(userId);
				cacheService.SetCache(userRoleIds, cacheKey, cacheKeyService);
			}

			return userRoleIds;
		}

		private List<int> GetUserPermitIds(int userId, List<int> userRoleIds)
		{
			string cacheKey = $"user:{userId}:permits";
			List<int> userPermitIds = cacheService.GetCache<List<int>>(cacheKey);

			if (userPermitIds == null)
			{
				userPermitIds = coreService.GetUserPermitIds(userRoleIds);
				cacheService.SetCache(userPermitIds, cacheKey, cacheKeyService);
			}

			return userPermitIds;
		}

		private string GetMenuString(List<int> userRoleIds, List<int> userPermitIds)
		{
			string cacheKey = $"roles:{string.Join(".", userRoleIds)}:menu";
			string menuString = cacheService.GetCache<string>(cacheKey);

			if (menuString == null)
			{
				menuString = coreService.GetMenuString(userPermitIds, GetMenuHierarchy());
				cacheService.SetCache(menuString, cacheKey, cacheKeyService);
			}

			return menuString;
		}

		private List<Menu> GetMenuHierarchy()
		{
			string cacheKey = "menu:hierarchy";
			List<Menu> menuHierarchy = cacheService.GetCache<List<Menu>>(cacheKey);

			if (menuHierarchy == null)
			{
				menuHierarchy = coreService.GetMenuHierarchy();
				cacheService.SetCache(menuHierarchy, cacheKey, cacheKeyService);
			}

			return menuHierarchy;
		}

		protected void ShowAlertBox(AlertType alertType, string alertMessage)
        {
			string tempMessage = string.Empty;
			tempMessage += " " + (alertMessage ?? string.Empty);

            //// set alert
            //ViewBag.alertBox = new AlertBox(alertType, tempMessage);

            // set alert
            TempData["alertBox"] = JsonConvert.SerializeObject(new AlertBox(alertType, tempMessage));
        }

        protected string GetModelErrorMessage(ModelStateDictionary modelState)
        {
            return string.Join(" ", modelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
        }
		
		protected string GetInnerExceptionMessage(Exception ex)
        {
			Exception exception = ex;

            //get the message from the innermost exception
            while (exception.InnerException != null)
                exception = exception.InnerException;

            return exception.Message;
        }
    }
}