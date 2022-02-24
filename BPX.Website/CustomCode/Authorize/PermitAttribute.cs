using BPX.Domain.DbModels;
using BPX.Service;
using BPX.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
            ClaimsPrincipal user = context.HttpContext.User;
            string host = context.HttpContext.Request.Host.ToString().ToLower().Trim();

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
                    Claim currPTokenClaim = user.Claims.SingleOrDefault(c => c.Type.Equals("PToken"));

					if (currPTokenClaim != null)
					{
						string currPToken = currPTokenClaim.Value;
						string currRToken = new string(currPToken.ToCharArray().Reverse().ToArray());

                        ICoreService coreService = (ICoreService)context.HttpContext.RequestServices.GetService(typeof(ICoreService));
						int sessionCookieTimeout= Convert.ToInt32(coreService.GetConfiguration().GetSection("AppSettings").GetSection("SessionCookieTimeout").Value);

                        // get portal details
                        IPortalService portalService = coreService.GetPortalService();
                        Portal portal = portalService.GetRecordsByFilter(c => c.PToken.Equals(currPToken)).SingleOrDefault();

                        // get login details
                        ILoginService loginService = coreService.GetLoginService();
                        Login login = loginService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.RToken.Equals(currRToken)).SingleOrDefault();

						if (portal != null && login != null)
						{
							if (portal.LastAccessTime < DateTime.Now.AddMinutes(-sessionCookieTimeout))
							{
								// force logout
								portal.PToken = Guid.NewGuid().ToString();

								portalService.UpdateRecord(portal);
								portalService.SaveDBChanges();
							}
							else
							{
                                // get user details
                                IUserService userService = coreService.GetUserService();
                                User currUser = userService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.PortalUUId.Equals(portal.PortalUUId) && c.LoginUUId.Equals(login.LoginUUId)).SingleOrDefault();

								if (currUser != null)
								{                   
									// SECURITY SECURITY SECURITY
									// verify the user :: portal :: login chain using currPToken on every request
									int currUserId = currUser.UserId;

                                    ICacheService cacheService = coreService.GetCacheService();
                                    ICacheKeyService cacheKeyService = coreService.GetCacheKeyService();

									string cacheKeyName = string.Empty;
									
									// userRoleIds
									cacheKeyName = $"user:{currUserId}:roles";
                                    List<int> userRoleIds = cacheService.GetCache<List<int>>(cacheKeyName);

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