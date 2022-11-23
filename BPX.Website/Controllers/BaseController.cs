using BPX.Domain.DbModels;
using BPX.Service;
using BPX.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace BPX.Website.Controllers
{
    public class BaseController<T> : Controller where T : class
    {
        // protected vars 
        protected readonly ILogger<T> logger;
        protected readonly ICoreService coreService;
        protected readonly int bpxPageSize;
        protected User currUser;                            // do not set "currUser" to readonly as we will be hydrating this object in the flow
        // private vars
        private readonly ICacheService cacheService;
        private readonly ICacheKeyService cacheKeyService;
        private readonly IErrorService errorService;

        public BaseController(ILogger<T> logger, ICoreService coreService)
        {
            this.logger = logger;
            this.coreService = coreService;
            this.bpxPageSize = Convert.ToInt32(coreService.GetConfiguration().GetSection("AppSettings").GetSection("PageSize").Value);
            this.currUser = new User();
            this.cacheService = coreService.GetCacheService();
            this.cacheKeyService = coreService.GetCacheKeyService();
            this.errorService = coreService.GetErrorService();
        }

        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            ViewBag.currLoginMenuString = GetLoginMenuString(null);

            //Stopwatch watch = new System.Diagnostics.Stopwatch();
            //watch.Start();

            if (ctx.HttpContext != null)
            {
                if (ctx.HttpContext.User != null)
                {
                    if (ctx.HttpContext.User.Identity.IsAuthenticated)
                    {
                        Claim currSTokenClaim = ctx.HttpContext.User.Claims.SingleOrDefault(c => c.Type.Equals("SToken"));
                        Claim currLTokenClaim = ctx.HttpContext.User.Claims.SingleOrDefault(c => c.Type.Equals("LToken"));

                        if (currSTokenClaim != null)
                        {
                            // get current SToken and LToken value from claims
                            string currSToken = currSTokenClaim.Value;
                            string currLToken = currLTokenClaim.Value;

                            // get sesson details :: using SToken
                            ISessonService sessonService = coreService.GetSessonService();
                            Sesson sesson = sessonService.GetSessonByToken(currSToken);

                            // get login details :: using LToken
                            ILoginService loginService = coreService.GetLoginService();
                            Login login = loginService.GetLoginByToken(currLToken);

                            if (sesson != null && login != null)
                            {
                                // check if the current access is after the sessionCookieTimeout timespan
                                if (sesson.LastAccessTime < DateTime.Now.AddMinutes(-sessionCookieTimeout))
                                {
                                    // force logout
                                    sesson.SToken = Guid.NewGuid().ToString();
                                    sessonService.UpdateRecordDapper(sesson);

                                    ctx.Result = new RedirectToRouteResult(
                                        new RouteValueDictionary
                                        {
                                            { "controller", "Account" },
                                            { "action", "Login" }
                                        }
                                    );
                                }
                                else
                                {
                                    // get user details :: using (SToken) SessonUUId :: using (LToken) LoginUUId
                                    IUserService userService = coreService.GetUserService();
                                    currUser = userService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())
                                                                            && c.SessonUUId.Equals(sesson.SessonUUId)
                                                                            && c.LoginUUId.Equals(login.LoginUUId))
                                                                            .SingleOrDefault();

                                    if (currUser != null)
                                    {
                                        // SECURITY SECURITY SECURITY
                                        // verify the user :: sesson :: login ..... chain using currSToken and currLToken on every request
                                        int currUserId = currUser.UserId;

                                        // get userRoles, userPermits, menu, and breadcrumb data
                                        List<int> currUserRoleIds = GetUserRoleIds(currUserId);                                             // userRoleIds
                                        List<int> currUserPermitIds = GetUserPermitIds(currUserId, currUserRoleIds);                        // userPermitIds
                                        string currLoginMenuString = GetLoginMenuString(currUser);                                          // loginMenuString
                                        List<Menu> currMenuHierarchy = GetMenuHierarchy(RecordStatus.Active.ToUpper(), "URL");              // menuHierarchy
                                        string currMenuString = GetMenuString(currUserRoleIds, currUserPermitIds, currMenuHierarchy);       // menuString
                                        string currBreadcrump = GetBreadCrumb(ctx, currMenuHierarchy);                                      // breadcrumb

                                        // populate ViewBag with user, userRoles, userPermits, menu, and breadcrumb data
                                        ViewBag.currUser = currUser;
                                        ViewBag.currUserRoleIds = currUserRoleIds;
                                        ViewBag.currUserPermitIds = currUserPermitIds;
                                        ViewBag.currLoginMenuString = currLoginMenuString;
                                        ViewBag.currMenuString = currMenuString;
                                        ViewBag.currBreadcrump = currBreadcrump;

                                        // log levels
                                        // Trace, Debug, Info, Warn, Error, Critical

                                        //logger.LogTrace("Tracing currUserId = " + currUserId);      // not wrking
                                        //logger.LogDebug("Debug currUserId = " + currUserId);        // not working
                                        //logger.LogInformation("Information currUserId = " + currUserId);
                                        //logger.LogWarning("Warning currUserId = " + currUserId);
                                        //logger.LogError("Error currUserId = " + currUserId);
                                        //logger.LogCritical("Critical currUserId = " + currUserId);

                                        // update the lastAccessTime in sesson
                                        sesson.LastAccessTime = DateTime.Now;
                                        sessonService.UpdateRecordDapper(sesson);

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
                                        //	ViewBag.currUserPermitIds = tempUserPermitIds;
                                        //}
                                        ////// END
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //watch.Stop();
            //string executionTime = "[milli: " + watch.ElapsedMilliseconds.ToString() + " ms] ......... ";
            //double elapsedTime = (double)watch.ElapsedTicks / (double)Stopwatch.Frequency;
            //executionTime += "[micro: " + (elapsedTime * 1000000).ToString("F2") + " us]";
            //ShowAlertBox(AlertType.Info, $"Execution Time: ......... {executionTime}");

            base.OnActionExecuting(ctx);
        }

        private List<int> GetUserRoleIds(int userId)
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

        private List<int> GetUserPermitIds(int userId, List<int> userRoleIds)
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

        private string GetBreadCrumb(ActionExecutingContext ctx, List<Menu> menuHierarchy)
        {
            string currArea = ctx.HttpContext.Request.RouteValues["area"] != null ? ctx.HttpContext.Request.RouteValues["area"].ToString() : string.Empty;
            string currController = ctx.HttpContext.Request.RouteValues["controller"] != null ? ctx.HttpContext.Request.RouteValues["controller"].ToString() : string.Empty;
            string currAction = ctx.HttpContext.Request.RouteValues["action"] != null ? ctx.HttpContext.Request.RouteValues["action"].ToString() : string.Empty;
            //string currId = ctx.HttpContext.Request.RouteValues["id"] != null ? ctx.HttpContext.Request.RouteValues["id"].ToString() : string.Empty;

            string lookupURL = ResolveURL($"~/{currArea}/{currController}").ToUpper();

            if (currController.ToUpper().Equals("HOME"))
            {
                lookupURL = lookupURL.Replace("/HOME", string.Empty);
            }

            int menuId = menuHierarchy.Where(c => c.MenuURL.ToUpper().Equals(lookupURL.ToUpper())).Select(c => c.MenuId).SingleOrDefault();

            if (menuId <= 0)
            {
                return "<li class=\"breadcrumb-item\">cannot</li><li class=\"breadcrumb-item\">generate</li><li class=\"breadcrumb-item\">breadcrumb</li>";
            }

            string cacheKeyName = $"menu:{menuId}:breadcrumb";
            string breadcrumb = cacheService.GetCache<string>(cacheKeyName, errorService);

            if (breadcrumb == null)
            {
                List<Menu> listBreadcrumb = coreService.GetBreadcrumbList(menuId, currController);

                // ResolveURL in the menu
                foreach (var itemBreadcrumb in listBreadcrumb)
                {
                    itemBreadcrumb.MenuURL = ResolveURL(itemBreadcrumb.MenuURL);
                }

                breadcrumb = string.Empty;

                foreach (Menu itemBreadcrumb in listBreadcrumb)
                {
                    if (itemBreadcrumb.MenuURL.Equals("/"))
                    {
                        breadcrumb += $"<li class=\"breadcrumb-item\"><a href=\"{itemBreadcrumb.MenuURL}\" class=\"text-decoration-none\"><span class=\"fa fa-home\">&nbsp;</span>{itemBreadcrumb.MenuName}</a></li>";
                    }
                    else
                    {
                        if (currController.ToUpper().Equals("HOME"))
                        {
                            breadcrumb += $"<li class=\"breadcrumb-item\">{itemBreadcrumb.MenuName}</li>";
                        }
                        else
                        {
                            breadcrumb += $"<li class=\"breadcrumb-item\"><a href=\"{itemBreadcrumb.MenuURL}\" class=\"text-decoration-none\">{itemBreadcrumb.MenuName}</a></li>";
                        }
                    }
                }

                cacheService.SetCache(breadcrumb, cacheKeyName, cacheKeyService);
            }

            breadcrumb += $"<li class=\"breadcrumb-item active\">{currAction}</li>";

            return breadcrumb;
        }

        private string GetLoginMenuString(User user)
        {
            if (user == null)
            {
                return $"<li><a class=\"nav-link\" href=\"{ResolveURL("~/Identity/Account/Register")}\">Register</a></li><li><a class=\"nav-link\" href=\"{ResolveURL("~/Identity/Account/Login")}\">Login</a></li>";
            }
            else
            {
                return $"<li><a class=\"nav-link\" href=\"{ResolveURL("~/Identity/Account/ChangePassword")}\">Hello {user.FirstName} {user.LastName}!</a></li><li><a class=\"nav-link\" href=\"{ResolveURL("~/Identity/Account/LogOff")}\">Logout</a></li>";
            }
        }

        private string GetMenuString(List<int> userRoleIds, List<int> userPermitIds, List<Menu> menuHierarchy)
        {
            string cacheKeyName = $"roles:{string.Join(".", userRoleIds)}:menu";
            string menuString = cacheService.GetCache<string>(cacheKeyName, errorService);

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
            List<Menu> menuHierarchy = cacheService.GetCache<List<Menu>>(cacheKeyName, errorService);

            if (menuHierarchy == null)
            {
                menuHierarchy = coreService.GetMenuHierarchy(statusFlag, orderBy);

                // ResolveURL in the menu
                foreach (var itemMenu in menuHierarchy)
                {
                    itemMenu.MenuURL = ResolveURL(itemMenu.MenuURL);
                }

                cacheService.SetCache(menuHierarchy, cacheKeyName, cacheKeyService);
            }

            return menuHierarchy;
        }

        protected string ResolveURL(string url)
        {
            return Url.Content(url);
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