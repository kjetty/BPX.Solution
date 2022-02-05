using BPX.DAL.Context;
using BPX.Service;
using BPX.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPX.Website.CustomCode.Authorize
{
	public class PermitTwoAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>, IAuthorizationHandler
    {
        private readonly BPXDbContext dbContext;
        private ILoginService loginService;
        private IUserService userService;
        private IRolePermitService rolePermitService;

        public PermitTwoAuthorizationHandler(BPXDbContext dbContext, ILoginService loginService, IUserService userService, IRolePermitService rolePermitService)
        {
            this.dbContext = dbContext;
            this.loginService = loginService;
            this.userService = userService;
            this.rolePermitService = rolePermitService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            bool found = false;

            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            // allow SuperAdmin unrestricted access
            if (context.User.IsInRole("1"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (requirement.AllowedRoles == null || requirement.AllowedRoles.Any() == false)
            {
                // it means any logged in user is allowed to access the resource
                found = true;
            }
            else
            {
                //this is the passed in PermitId :: [Authorize(Roles = Permits.Identity.User.List)]
                var permitListIds = requirement.AllowedRoles.Select(int.Parse).ToList();
                var claimLoginId = context.User.Claims.FirstOrDefault(c => c.Type.Equals("LoginId")).Value;
                var claimLoginToken = context.User.Claims.FirstOrDefault(c => c.Type.Equals("LoginToken")).Value;
                var claimUserId = context.User.Claims.FirstOrDefault(c => c.Type.Equals("UserId")).Value;
                int userId = Convert.ToInt32(claimUserId);

                // get user (from DB)
                var login = loginService.GetRecordById(userId);

                if (login != null)
                {
                    // verify if the current request is valid or not
                    // loginID and current SessionUUID must match
                    if (claimLoginId.ToUpper().Equals(login.LoginId.ToUpper()) && claimLoginToken.Equals(login.LoginToken))
                    {
                        // get ROLES associates with the PERMIT (from DB)
                        var permitRolesList = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && permitListIds.Contains(c.PermitId)).Select(c => c.RoleId).Distinct().ToList();

                        // get ROLES associated with the USER (from CLAIM)
                        var userRolesList = context.User.Claims.Where(c => c.Type.Equals(ClaimTypes.Role)).Select(c => Convert.ToInt32(c.Value)).ToList();

                        // intersect to check for any matching ROLES
                        if (userRolesList.Any(x => permitRolesList.Any(y => y == x)))
                        {
                            found = true;
                        }
                    }
                }
            }

            if (found)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}