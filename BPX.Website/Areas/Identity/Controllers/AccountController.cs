using BPX.Domain.CustomModels;
using BPX.Domain.DbModels;
using BPX.Domain.ViewModels;
using BPX.Service;
using BPX.Utils;
using BPX.Website.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace BPX.Website.Areas.Identity.Controllers
{
	[Area("Identity")]
    public class AccountController : BaseController<AccountController>
    {
        private readonly ILoginService loginService;
        private readonly IUserService userService;
        private readonly IUserRoleService userRoleService;

        public AccountController(ILoginService loginService, IUserService userService, IUserRoleService userRoleService)
        {
            this.loginService = loginService;
            this.userService = userService;
            this.userRoleService = userRoleService;
        }

        // GET: /Identity/Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;

            return View();
        }

        // POST: /Identity/Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(BPXLoginViewModel model)
        {
            var login = loginService.GetRecordsByFilter(c => c.StatusFlag == RecordStatus.Active && c.LoginId == model.LoginId).FirstOrDefault();
            bool passwordIsVerified = false;

            if (login == null)
            {
                // set alert
                ShowAlert(AlertType.Warning, "Login failed. Please try again. ");

                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            if (login.UserId <= 0)
            {
                // set alert
                ShowAlert(AlertType.Warning, "Login failed. Try again.");

                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            try
            {
                // verify password
                var result = new PasswordHasher<Login>().VerifyHashedPassword(login, login.PasswordHash, model.Password);
                if (result == PasswordVerificationResult.Success) passwordIsVerified = true;
                else if (result == PasswordVerificationResult.SuccessRehashNeeded) passwordIsVerified = true;
                else if (result == PasswordVerificationResult.Failed) passwordIsVerified = false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message + ", " + ex.StackTrace);
            }

            ////// Developer Override for Password
            ////// OverrideOverrideOverride 
            ////// use for testing only
            ////// comment befor publishing
            ////// START
            //if (currRequestMeta.host.Contains("localhost"))
            //{
            //    if (model.Password.Equals("password"))
            //    {
            //        passwordIsVerified = true;
            //    }
            //}
            ////// END

			if (!passwordIsVerified)
            {
                // set alert
                ShowAlert(AlertType.Warning, "Login failed. Try again.");

                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // generate new LoginToken on every login
            login.LoginToken = Guid.NewGuid().ToString();
            login.LastLoginDate = DateTime.Now;
            
            // save changes
            loginService.SaveDBChanges();

            // get user and userRoles
            var user = userService.GetRecordByID(login.UserId);
            var userRolesIds = userRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.UserId.Equals(login.UserId)).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();

            string lastName = user.LastName ?? string.Empty;
			string firstName = user.FirstName ?? string.Empty;
			string fullName = firstName + " " + lastName;

			//var claims = new List<Claim>
			//{
			//    //new Claim("LoginId", login.LoginId != null ? login.LoginId : "InvalidLoginId"),
			//    new Claim("currLoginToken", login.LoginToken ?? "Invalid Login Token"),
			//    //new Claim("UserId", user.UserId > 0 ? user.UserId.ToString() : "-999"),
			//    new Claim("FullName", fullName),
			//    new Claim("LastName", lastName),
			//    new Claim("FirstName", firstName),
			//    new Claim("Email", user.Email ?? string.Empty),
			//    new Claim("Mobile", user.Mobile ?? string.Empty),
			//    new Claim(ClaimTypes.Name, fullName),
			//    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
			//    //new Claim(ClaimTypes.Sid, login.LoginId ?? "InvalidLoginId")
			//};
			
			var claims = new List<Claim>
			{
                new Claim("currLoginToken", login.LoginToken ?? "Invalid BPX Login Token"),
                new Claim(ClaimTypes.Name, fullName),
            };

			foreach (var userRoleId in userRolesIds)
			{
				claims.Add(new Claim(ClaimTypes.Role, userRoleId.ToString()));
			}

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                // AllowRefresh = true,
                // IssuedUtc = <DateTimeOffset>,
                // RedirectUri = <string>,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                IsPersistent = false
            };

            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home", new { area = "" });            
        }

        // GET: /Identity/Account/Denied
        [AllowAnonymous]
        public ActionResult Denied()
        {
            return View();
        }

        // GET: /Identity/Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Identity/Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel collection)
        {
            try
            {
                // return if model is invalid
                if (!ModelState.IsValid)
                {
                    string modelErrorMessage = GetModelErrorMessage(ModelState);
                    ModelState.AddModelError("", modelErrorMessage);

                    // set alert
                    ShowAlert(AlertType.Error, modelErrorMessage);

                    return View(collection);
                }

                // check if password and confirmPassword match
                if (!collection.Password.Equals(collection.ConfirmPassword))
                {
                    string errorMessage = "The password and confirmation password do not match";
                    ModelState.AddModelError("", errorMessage);

                    // set alert
                    ShowAlert(AlertType.Error, errorMessage);

                    return View(collection);
                }

                // check if LoginId exists
                var duplicateLoginList = loginService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.LoginId.ToUpper().Equals(collection.LoginId.Trim().ToUpper())).ToList();

                if (duplicateLoginList.Count == 0)
                {
                    User user = new()
                    {
                        // set core data
                        FirstName = collection.FirstName,
                        LastName = collection.FirstName,
                        Email = collection.FirstName,
                        Mobile = collection.FirstName,
                        // set generic data
                        StatusFlag = RecordStatus.Active,
                        ModifiedBy = 1,
                        ModifiedDate = DateTime.Now
                    };

                    userService.InsertRecord(user);
                    userService.SaveDBChanges();

                    Login login = new()
                    {
                        // set core data
                        UserId = user.UserId,
                        LoginId = collection.LoginId
                    };

                    // hash the password
                    var passwordHasher = new PasswordHasher<Login>();
                    string hashedPassword = passwordHasher.HashPassword(login, collection.Password);

                    login.PasswordHash = hashedPassword;
                    login.LoginToken = Guid.NewGuid().ToString();
                    login.LastLoginDate = DateTime.Now.AddMinutes(-60);
                    // set generic data
                    login.StatusFlag = RecordStatus.Active;
                    login.ModifiedBy = 1;
                    login.ModifiedDate = DateTime.Now;

                    loginService.InsertRecord(login);
                    loginService.SaveDBChanges();

                    // set alert
                    ShowAlert(AlertType.Success, "Login is successfully created.");

                    return RedirectToAction(nameof(Login), new { controller = "Account" });
                }
                else
                {
                    // set alert box
                    ShowAlert(AlertType.Warning, "Login ID is already taken. Please try another name.");

                    return View(collection);
                }
            }
            catch (Exception ex)
            {
                // prepare data
                
                string errorStackTrace = ex.StackTrace.ToString();
                string errorMessage = GetGarneredErrorMessage(ex);

                // log
                logger.Log(LogLevel.Error, errorMessage + " " + errorStackTrace);

                // set alert
                ShowAlert(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Register));
            }
        }

        // GET: /Identity/Account/ChangePassword
        [AllowAnonymous]
        //[Permit(Permits.Identity.Login.ChangePassword)]
        public ActionResult ChangePassword()
        {
            ChangePasswordViewModel model = new();

            return View(model);
        }

        // POST: /Identity/Account/ChangePassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //[Permit(Permits.Identity.Login.ChangePassword)]
        public ActionResult ChangePassword(ChangePasswordViewModel collection)
        {
            //collection.

            try
            {
                // return if model is invalid
                if (!ModelState.IsValid)
                {
                    string modelErrorMessage = GetModelErrorMessage(ModelState);
                    ModelState.AddModelError("", modelErrorMessage);

                    // set alert
                    ShowAlert(AlertType.Error, modelErrorMessage);

                    return View(collection);
                }

                // check if password and confirmPassword match
                if (!collection.NewPassword.Equals(collection.ConfirmPassword))
                {
                    string errorMessage = "The password and confirmation password do not match";
                    ModelState.AddModelError("", errorMessage);

                    // set alert
                    ShowAlert(AlertType.Error, errorMessage);

                    return View(collection);
                }

                if (currUserMeta.UserId != collection.UserId)
				{
                    string errorMessage = "Invalid user";
                    ModelState.AddModelError("", errorMessage);

                    // set alert
                    ShowAlert(AlertType.Error, errorMessage);

                    return View(collection);
                }

                // todo :: verify old password

                // get user
                var login = loginService.GetRecordByID(currUserMeta.UserId);

				// hash the password
				var passwordHasher = new PasswordHasher<Login>();
				string hashedPassword = passwordHasher.HashPassword(login, collection.NewPassword);

                login.PasswordHash = hashedPassword;

                loginService.SaveDBChanges();

                // set alert
                ShowAlert(AlertType.Success, "Password is successfully changed.");

                return RedirectToAction(nameof(Login), new { controller = "Account" });

            }
            catch (Exception ex)
            {
                // prepare data
                
                string errorStackTrace = ex.StackTrace.ToString();
                string errorMessage = GetGarneredErrorMessage(ex);

                // log
                logger.Log(LogLevel.Error, errorMessage + " " + errorStackTrace);

                // set alert
                ShowAlert(AlertType.Error, errorMessage);

                //return RedirectToAction(nameof(Register));
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        // POST: /Identity/Account/LogOff
        public ActionResult LogOff()
        {
            if (currUserMeta.UserId > 0)
            {
                var login = loginService.GetRecordByID(currUserMeta.UserId);

                login.LoginToken = Guid.NewGuid().ToString();
                login.LastLoginDate = DateTime.Now;
                
                loginService.SaveDBChanges();
            }

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (currUserMeta.UserId > 0)
            {
                // set alert
                ShowAlert(AlertType.Info, "User is successfully logged off.");
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}