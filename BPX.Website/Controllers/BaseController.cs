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
		protected List<Menu> currMenuHierarchy;
		protected string currBreadcrump;
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

								// get menuHierarchy
								currMenuHierarchy = GetMenuHierarchy(RecordStatus.Active, "url");

								// get menuString
								currMenuString = GetMenuString(currUserMeta.UserRoleIds, currUserMeta.UserPermitIds, currMenuHierarchy);

								// get breadcrumb
								currBreadcrump = GetBreadCrumb(ctx, currMenuHierarchy);

								// populate ViewBag
								ViewBag.currMenuString = currMenuString;
								ViewBag.currBreadcrump = currBreadcrump;
								ViewBag.currUserPermitIds = currUserMeta.UserPermitIds;

								////// Developer Override for Permits - BaseController (Part A) + PermitAttribute (PartB)
								////// OverrideOverrideOverride 
								////// use for testing only
								////// comment before publishing
								////// START
								//if (ctx.HttpContext.Request.Host.Value.Contains("localhost"))
								//{
								//	List<int> tempUserPermitIds = new List<int>();
								//	for (int i = 0; i < 10000; i++)
								//	{
								//		tempUserPermitIds.Add(i);
								//	}
								//	currUserMeta.UserPermitIds = tempUserPermitIds;
								//	ViewBag.currUserPermitIds = currUserMeta.UserPermitIds;
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
			string cacheKeyName = $"user:{userId}:meta";
			UserMeta userMeta = cacheService.GetCache<UserMeta>(cacheKeyName);

			if (userMeta == null)
			{
				userMeta = coreService.GetUserMeta(userId);
				cacheService.SetCache(userMeta, cacheKeyName, cacheKeyService);
			}

			return userMeta;
		}

		private List<int> GetUserRoleIds(int userId)
		{
			string cacheKeyName = $"user:{userId}:roles";
			List<int> userRoleIds = cacheService.GetCache<List<int>>(cacheKeyName);

			if (userRoleIds == null)
			{
				userRoleIds = coreService.GetUserRoleIds(userId);
				cacheService.SetCache(userRoleIds, cacheKeyName, cacheKeyService);
			}

			return userRoleIds;
		}

		private List<int> GetUserPermitIds(int userId, List<int> userRoleIds)
		{
			string cacheKeyName = $"user:{userId}:permits";
			List<int> userPermitIds = cacheService.GetCache<List<int>>(cacheKeyName);

			if (userPermitIds == null)
			{
				userPermitIds = coreService.GetUserPermitIds(userRoleIds);
				cacheService.SetCache(userPermitIds, cacheKeyName, cacheKeyService);
			}

			return userPermitIds;
		}

		private string GetBreadCrumb(ActionExecutingContext ctx, List<Menu> menuHierarchy)
		{
			string currArea = ctx.HttpContext.Request.RouteValues["area"] != null ? ctx.HttpContext.Request.RouteValues["area"].ToString() : string.Empty;
			string currController = ctx.HttpContext.Request.RouteValues["controller"] != null ? ctx.HttpContext.Request.RouteValues["controller"].ToString() : string.Empty;
			string currAction = ctx.HttpContext.Request.RouteValues["action"] != null ? ctx.HttpContext.Request.RouteValues["action"].ToString() : string.Empty;
			//string currId = ctx.HttpContext.Request.RouteValues["id"] != null ? ctx.HttpContext.Request.RouteValues["id"].ToString() : string.Empty;
			
			string lookupURL = $"/{currArea}/{currController}";
			int menuId = menuHierarchy.Where(c => c.MenuURL.Equals(lookupURL)).Select(c => c.MenuId).SingleOrDefault();

			if (menuId <= 0)
			{
				return "<li class=\"breadcrumb-item\">cannot</li><li class=\"breadcrumb-item\">generate</li><li class=\"breadcrumb-item\">breadcrumb</li>";
			}

			string cacheKeyName = $"menu:{menuId}:breadcrumb";
			string breadcrumb = cacheService.GetCache<string>(cacheKeyName);

			if (breadcrumb == null)
			{
				breadcrumb = coreService.GetBreadcrumb(menuId);
				cacheService.SetCache(breadcrumb, cacheKeyName, cacheKeyService);
			}

			return breadcrumb += $"<li class=\"breadcrumb-item active\">{currAction}</li>";
		}

		private string GetMenuString(List<int> userRoleIds, List<int> userPermitIds, List<Menu> menuHierarchy)
		{
			string cacheKeyName = $"roles:{string.Join(".", userRoleIds)}:menu";
			string menuString = cacheService.GetCache<string>(cacheKeyName);

			if (menuString == null)
			{
				menuString = coreService.GetMenuString(userPermitIds, menuHierarchy);
				cacheService.SetCache(menuString, cacheKeyName, cacheKeyService);
			}

			return menuString;
		}

		private List<Menu> GetMenuHierarchy(string statusFlag, string orderBy)
		{
			string cacheKeyName = $"menu:hierarchy";
			List<Menu> menuHierarchy = cacheService.GetCache<List<Menu>>(cacheKeyName);

			if (menuHierarchy == null)
			{
				menuHierarchy = coreService.GetMenuHierarchy(statusFlag, orderBy);
				cacheService.SetCache(menuHierarchy, cacheKeyName, cacheKeyService);
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