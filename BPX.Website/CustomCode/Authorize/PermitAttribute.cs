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
        private readonly int _permitID;

        public PermitAttribute(int permitID = 0)
        {
            _permitID = permitID;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var success = false;
            var user = context.HttpContext.User;
            var host = context.HttpContext.Request.Host.ToString();

            if (user != null && user.Identity.IsAuthenticated)
            {
                // any logged in user is allowed to access the resource
                // for [Permit] attribute without the permit parameter
                if (_permitID == 0)
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

            //// Developer Override
            //// Development Override
            //// OverrideOverrideOverride :: todo
            //// comment out this code before publishing for PRODUCTION RELEASE
            //if (host.Contains("localhost"))
            //{
            //    string developerPermitAttributeOverride = Startup.Configuration.GetSection("DeveloperOverride").GetSection("PermitAttributeOverride").Value.ToString().Trim();

            //    if (developerPermitAttributeOverride.Equals("YES-ForcedSet"))
	           // {
            //        // Development Override :: use for testing only
            //        success = true;
            //    }                
            //}

			if (!success)
            {
                // fetch services
                var accountService = (ILoginService)context.HttpContext.RequestServices.GetService(typeof(ILoginService));
                var rolePermitService = (IRolePermitService)context.HttpContext.RequestServices.GetService(typeof(IRolePermitService));
                var userRoleService = (IUserRoleService)context.HttpContext.RequestServices.GetService(typeof(IUserRoleService));
                var memoryCacheKeyService = (IMemoryCacheKeyService)context.HttpContext.RequestServices.GetService(typeof(IMemoryCacheKeyService));
                var bpxCache = (IBPXCache)context.HttpContext.RequestServices.GetService(typeof(IBPXCache));

                // get data from claims
                var claimLogintoken = user.Claims.FirstOrDefault(c => c.Type == "currLoginToken").Value;

                // get user (from DB)
                var account = accountService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.LoginToken.Equals(claimLogintoken)).SingleOrDefault();

                if (account != null)
                {
                    int userID = account.UserId;
                    string cacheKey = string.Empty;

                    cacheKey = $"user:{userID}:roles";
                    var listUserRoles = bpxCache.GetCache<List<int>>(cacheKey);

                    if (listUserRoles == null)
                    {
                        listUserRoles = userRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.UserId == userID).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
                        bpxCache.SetCache(listUserRoles, cacheKey, memoryCacheKeyService);
                    }

                    //listUserRolesPermits
                    cacheKey = $"permit:{_permitID}:roles";
                    var listPermitRoles = bpxCache.GetCache<List<int>>(cacheKey);

                    if (listPermitRoles == null)
                    {
                        listPermitRoles = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.PermitID == _permitID).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();

                        bpxCache.SetCache(listPermitRoles, cacheKey, memoryCacheKeyService);
                    }

                    // intersect to check for any matching ROLES
                    if (listUserRoles.Any(x => listPermitRoles.Any(y => y == x)))
                    {
                        success = true;
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