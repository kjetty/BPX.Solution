using BPX.Service;
using BPX.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPX.Website.CustomCode.Authorize
{
	[AttributeUsage(AttributeTargets.Class |
							 AttributeTargets.Method
						   , AllowMultiple = true
						   , Inherited = true)]
	public class PermitAttribute : AuthorizeAttribute, IAuthorizationFilter
	{
		private readonly int permitId;

		public PermitAttribute(int permitId = 0)
		{
			this.permitId = permitId;
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var success = false;
			var user = context.HttpContext.User;
			var host = context.HttpContext.Request.Host.ToString().ToLower().Trim();

			if (user != null && user.Identity.IsAuthenticated)
			{
				// any logged in user is allowed to access the resource
				// for [Permit] attribute without the permit parameter
				if (permitId == 0)
				{
					success = true;
				}
			}
			else
			{
				// redirect to forbidden page
				context.Result = new ForbidResult();
				return;
			}

			//// for future use, using roles
			//if (user.IsInRole("1"))
			//{
			//}

			////// Developer Override for Permits - BaseController (Part A) + PermitAttribute (PartB)
			////// OverrideOverrideOverride 
			////// use for testing only
			////// comment befor publishing
			////// START
			//if (host.Contains("localhost"))
			//{
			//	success = true;
			//}
			////// END

			if (!success)
			{
				var claimCurrLoginToken = user.Claims.FirstOrDefault(c => c.Type == "currLoginToken");

				if (claimCurrLoginToken != null)
				{
					var coreService = (ICoreService)context.HttpContext.RequestServices.GetService(typeof(ICoreService));
					int userId = coreService.GetUserId(claimCurrLoginToken.Value);

					if (userId > 0)
					{	
						var cacheKey = string.Empty;
						var cacheService = coreService.GetCacheService();
						var cacheKeyService = coreService.GetCacheKeyService();

						// userRoleIds
						cacheKey = $"user:{userId}:roles";
						var userRoleIds = cacheService.GetCache<List<int>>(cacheKey);

						if (userRoleIds == null)
						{
							userRoleIds = coreService.GetUserRoleIds(userId);
							coreService.GetCacheService().SetCache(userRoleIds, cacheKey, cacheKeyService);
						}

						// permitRoleIds
						cacheKey = $"permit:{permitId}:roles";
						var permitRoleIds = cacheService.GetCache<List<int>>(cacheKey);

						if (permitRoleIds == null)
						{
							permitRoleIds = coreService.GetPermitRoles(permitId);
							coreService.GetCacheService().SetCache(permitRoleIds, cacheKey, cacheKeyService);
						}

						// intersect to check for any matching ROLES
						if (permitRoleIds.Any(x => userRoleIds.Any(y => y == x)))
						{
							success = true;
						}
					}
				}
			}

			if (!success)
			{
				// redirect to forbidden page
				context.Result = new ForbidResult();
				return;
			}

			return;
		}
	}
}