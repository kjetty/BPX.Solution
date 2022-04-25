using BPX.Domain.DbModels;
using BPX.Service;
using BPX.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

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
            bool success = false;
            ClaimsPrincipal principalUser = context.HttpContext.User;
            string host = context.HttpContext.Request.Host.ToString().ToLower().Trim();

			if (principalUser == null)
			{
				// redirect to forbidden page
				context.Result = new ForbidResult();
				return;
			}

			if (!principalUser.Identity.IsAuthenticated)
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

			if (principalUser.Identity.IsAuthenticated)
			{
				if (permitId.Equals(0))
				{
					// to handle [Permit] attribute without the permit parameter
					// any logged in user is allowed to access the resource
					success = true;
				}

				if (permitId > 0)
				{
                    Claim currPTokenClaim = principalUser.Claims.SingleOrDefault(c => c.Type.Equals("PToken"));

					if (currPTokenClaim != null)
					{
						string currPToken = currPTokenClaim.Value;
						string currLToken = Utility.GetLToken(currPToken);

                        ICoreService coreService = (ICoreService)context.HttpContext.RequestServices.GetService(typeof(ICoreService));
						int sessionCookieTimeout= Convert.ToInt32(coreService.GetConfiguration().GetSection("AppSettings").GetSection("SessionCookieTimeout").Value);

						// get portal details :: using PToken
						IPortalService portalService = coreService.GetPortalService();
						Portal portal = portalService.GetPortalByToken(currPToken);

						// get login details :: using RToken
						ILoginService loginService = coreService.GetLoginService();
                        Login login = loginService.GetLoginByToken(currLToken);

						if (portal != null && login != null)
						{
							if (portal.LastAccessTime < DateTime.Now.AddMinutes(-sessionCookieTimeout))
							{
								// force logout
								portal.PToken = Guid.NewGuid().ToString();

								//portalService.UpdateRecord(portal);
								//portalService.SaveDBChanges();

								portalService.UpdateRecordDapper(portal);

								context.Result = new RedirectToRouteResult(
									new RouteValueDictionary
									{
										{ "controller", "Account" },
										{ "action", "Login" }
									}
								);
							}
							else
							{
								// get user details :: uisng (PToken) PortalUUId :: using (RToken) LoginUUId + UserUUId
								IUserService userService = coreService.GetUserService();
								User currUser = userService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) 
																				&& c.PortalUUId.Equals(portal.PortalUUId) 
																				&& c.LoginUUId.Equals(login.LoginUUId))
																				.SingleOrDefault();

								if (currUser != null)
								{
									ICacheService cacheService = coreService.GetCacheService();
									ICacheKeyService cacheKeyService = coreService.GetCacheKeyService();
									IErrorService errorService = coreService.GetErrorService();
									
									// SECURITY SECURITY SECURITY
									// verify the user :: portal :: login chain using currPToken on every request
									int currUserId = currUser.UserId;
									List<int> userRoleIds = GetUserRoleIds(currUserId, coreService, cacheService, cacheKeyService, errorService);
									List<int> userPermitIds = GetUserPermitIds(currUserId, userRoleIds, coreService, cacheService, cacheKeyService, errorService);

									// check if user has the permit
									if (userPermitIds.Contains(permitId))
									{
										// allow access
										success = true;
									}
								}
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
		private List<int> GetUserRoleIds(int userId, ICoreService coreService, ICacheService cacheService, ICacheKeyService cacheKeyService, IErrorService errorService)
		{
            string cacheKeyName = $"user:{userId}:roles";
            List<int> userRoleIds = cacheService.GetCache<List<int>>(cacheKeyName, errorService);

            if (userRoleIds == null)
            {
                userRoleIds = coreService.GetUserRoleIds(userId);
                cacheService.SetCache(userRoleIds, cacheKeyName, cacheKeyService);
            }

			return userRoleIds;
        }

		private List<int> GetUserPermitIds(int userId, List<int> userRoleIds, ICoreService coreService, ICacheService cacheService, ICacheKeyService cacheKeyService, IErrorService errorService)
        {
            string cacheKeyName = $"user:{userId}:permits";
            List<int> userPermitIds = cacheService.GetCache<List<int>>(cacheKeyName, errorService);

            if (userPermitIds == null)
            {
                userPermitIds = coreService.GetUserPermitIds(userRoleIds);
                cacheService.SetCache(userPermitIds, cacheKeyName, cacheKeyService);
            }

			return userPermitIds;

		}
	}
}