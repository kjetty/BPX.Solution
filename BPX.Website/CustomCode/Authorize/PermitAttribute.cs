using BPX.Service;
using BPX.Utils;
using BPX.Website.CustomCode.Cache;
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
					// get current loginToken value
					string loginToken = claimCurrLoginToken.Value;

					// verify access (process A - using optinmed database calls)
					// avoid this call, as this function makes un-cached database calls and can be slow
					//var accountService = (AccountService)context.HttpContext.RequestServices.GetService(typeof(IAccountService));
					//success = accountService.IsUserPermitted(loginToken, permitId);

					//OR

					// verify access (process B - using cache and intersect function)
					var loginService = (ILoginService)context.HttpContext.RequestServices.GetService(typeof(ILoginService));
					var userRoleService = (IUserRoleService)context.HttpContext.RequestServices.GetService(typeof(IUserRoleService));
					var rolePermitService = (IRolePermitService)context.HttpContext.RequestServices.GetService(typeof(IRolePermitService));
					var bpxCache = (IBPXCache)context.HttpContext.RequestServices.GetService(typeof(IBPXCache));
					var CacheKeyService = (ICacheKeyService)context.HttpContext.RequestServices.GetService(typeof(ICacheKeyService));

					var login = loginService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.LoginToken.Equals(loginToken)).SingleOrDefault();

					if (login != null)
					{
						int userId = login.UserId;

						if (userId > 0)
						{
							string cacheKey = string.Empty;

							// userRoleIds
							cacheKey = $"user:{userId}:roles";
							var userRoleIds = bpxCache.GetCache<List<int>>(cacheKey);

							if (userRoleIds == null)
							{
								userRoleIds = userRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.UserId == userId).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
								bpxCache.SetCache(userRoleIds, cacheKey, CacheKeyService);
							}

							// permitRoleIds
							cacheKey = $"permit:{permitId}:roles";
							var permitRoleIds = bpxCache.GetCache<List<int>>(cacheKey);

							if (permitRoleIds == null)
							{
								permitRoleIds = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.PermitID == permitId).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
								bpxCache.SetCache(permitRoleIds, cacheKey, CacheKeyService);

								//note
								//permitRoleIds can be further refined to filter using userRoleIds, but then caching cannot be applied
								//we are resorting to get all roles per permit [permitRoleIds] so that we can use cache for performance
							}

							// intersect to check for any matching ROLES
							if (permitRoleIds.Any(x => userRoleIds.Any(y => y == x)))
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