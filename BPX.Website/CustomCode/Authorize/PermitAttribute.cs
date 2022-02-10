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

			if (user == null)
			{
				// redirect to forbidden page
				context.Result = new ForbidResult();
				return;
			}

			if (!user.Identity.IsAuthenticated)
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

			if (user.Identity.IsAuthenticated)
			{
				if (permitId.Equals(0))
				{
					// to handle [Permit] attribute without the permit parameter
					// any logged in user is allowed to access the resource
					success = true;
				}

				if (permitId > 0)
				{
					var currLoginTokenClaim = user.Claims.FirstOrDefault(c => c.Type.Equals("BPXLoginToken"));

					if (currLoginTokenClaim != null)
					{
						var coreService = (ICoreService)context.HttpContext.RequestServices.GetService(typeof(ICoreService));

						// get loginToken and associated userId
						string currLoginToken = currLoginTokenClaim.Value;
						int currUserId = coreService.GetUserId(currLoginToken);

						if (currUserId > 0)
						{
							string cacheKeyName = string.Empty;
							var cacheService = coreService.GetCacheService();
							var cacheKeyService = coreService.GetCacheKeyService();

							// userRoleIds
							cacheKeyName = $"user:{currUserId}:roles";
							var userRoleIds = cacheService.GetCache<List<int>>(cacheKeyName);

							if (userRoleIds == null)
							{
								userRoleIds = coreService.GetUserRoleIds(currUserId);
								cacheService.SetCache(userRoleIds, cacheKeyName, cacheKeyService);
							}

							// userPermitIds
							cacheKeyName = $"user:{currUserId}:permits";
							List<int> userPermitIds = cacheService.GetCache<List<int>>(cacheKeyName);

							if (userPermitIds == null)
							{
								userPermitIds = coreService.GetUserPermitIds(userRoleIds);
								cacheService.SetCache(userPermitIds, cacheKeyName, cacheKeyService);
							}

							// check if user has the permit
							if (userPermitIds.Contains(permitId))
							{
								success = true;
							}
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