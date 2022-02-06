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
				if (permitId.Equals(0))
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
				var currLoginToken = user.Claims.FirstOrDefault(c => c.Type.Equals("BPXLoginToken"));

				if (currLoginToken != null)
				{
					string loginToken = currLoginToken.Value;
					var coreService = (ICoreService)context.HttpContext.RequestServices.GetService(typeof(ICoreService));
					int userId = coreService.GetUserId(loginToken);

					if (userId > 0)
					{	
						string cacheKeyName = string.Empty;
						var cacheService = coreService.GetCacheService();
						var cacheKeyService = coreService.GetCacheKeyService();

						// userRoleIds
						cacheKeyName = $"user:{userId}:roles";
						var userRoleIds = cacheService.GetCache<List<int>>(cacheKeyName);

						if (userRoleIds == null)
						{
							userRoleIds = coreService.GetUserRoleIds(userId);
							coreService.GetCacheService().SetCache(userRoleIds, cacheKeyName, cacheKeyService);
						}

						// permitRoleIds
						cacheKeyName = $"permit:{permitId}:roles";
						var permitRoleIds = cacheService.GetCache<List<int>>(cacheKeyName);

						if (permitRoleIds == null)
						{
							permitRoleIds = coreService.GetPermitRoles(permitId);
							coreService.GetCacheService().SetCache(permitRoleIds, cacheKeyName, cacheKeyService);
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