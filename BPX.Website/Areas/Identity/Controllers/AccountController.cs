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
using System.Transactions;

namespace BPX.Website.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : BaseController<AccountController>
    {
        private readonly ISessonService sessonService;
        private readonly ILoginService loginService;
        private readonly IUserService userService;
        private readonly IUserRoleService userRoleService;

        public AccountController(ILogger<AccountController> logger, ICoreService coreService) : base(logger, coreService)
        {
            this.sessonService = coreService.GetSessonService();
            this.loginService = coreService.GetLoginService();
            this.userService = coreService.GetUserService();
            this.userRoleService = coreService.GetUserRoleService();
        }

        // GET: /Identity/Account/Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;

            return View();
        }

        // POST: /Identity/Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(BPXLoginViewModel model)
        {
            Login login = loginService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.LoginName.Equals(model.LoginName)).SingleOrDefault();
            bool passwordIsVerified = false;

            if (login == null)
            {
                // set alert
                ShowAlertBox(AlertType.Warning, "Login failed. Please try again.");

                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            if (!login.LoginType.ToUpper().Equals(LoginCategory.Username.ToUpper()))
            {
                // set alert
                ShowAlertBox(AlertType.Warning, "Login failed. You account is not set up to login using username. Please contact the Administrator. ");

                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            try
            {
                // verify password
                PasswordVerificationResult result = new PasswordHasher<Login>().VerifyHashedPassword(login, login.PasswordHash, model.Password);
                if (result == PasswordVerificationResult.Success) passwordIsVerified = true;
                else if (result == PasswordVerificationResult.SuccessRehashNeeded) passwordIsVerified = true;
                else if (result == PasswordVerificationResult.Failed) passwordIsVerified = false;
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "AccountController.082" + errorMessage);
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
                ShowAlertBox(AlertType.Warning, "Login failed. Try again.");

                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // get user
            User user = userService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.LoginUUId.Equals(login.LoginUUId)).SingleOrDefault();

            if (user == null)
            {
                // set alert
                ShowAlertBox(AlertType.Warning, "Login failed. Please try again. ");

                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            if (user.UserId <= 0)
            {
                ShowAlertBox(AlertType.Warning, "Login failed. Try again.");

                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            Sesson sesson = sessonService.GetRecordsByFilter(c => c.SessonUUId.Equals(user.SessonUUId)).SingleOrDefault();
        
            if (sesson == null)
            {
                // set alert
                ShowAlertBox(AlertType.Warning, "Login failed. Please try again. ");

                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // generate new sToken and lToken on every login
            string sToken = Guid.NewGuid().ToString();
            string lToken = Guid.NewGuid().ToString();

            // sesson
            sesson.SToken = sToken;
            sesson.LastAccessTime = DateTime.Now;

            // login
            login.LToken = lToken;
            login.LastLoginDate = DateTime.Now;
            login.ModifiedBy = user.UserId;
            login.ModifiedDate = DateTime.Now;

            using (TransactionScope scope = new TransactionScope())
            {
                loginService.UpdateRecord(login);
                loginService.SaveDBChanges();

                sessonService.UpdateRecord(sesson);
                sessonService.SaveDBChanges();

                scope.Complete();
            }

            string fullName = user.LastName ?? string.Empty + " " + user.FirstName ?? string.Empty;

            List<Claim> listClaims = new List<Claim>
            {
                new Claim("SToken", sesson.SToken ?? "Invalid SToken"),
                new Claim("LToken", login.LToken ?? "Invalid LToken"),
                new Claim(ClaimTypes.Name, fullName),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(listClaims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                // AllowRefresh = true,
                // IssuedUtc = <DateTimeOffset>,
                // RedirectUri = <string>,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(sessionCookieTimeout),
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
        public IActionResult Denied()
        {
            return View();
        }

        // GET: /Identity/Account/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Identity/Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel collection)
        {
            try
            {
                // return if model is invalid
                if (!ModelState.IsValid)
                {
                    string modelErrorMessage = GetModelErrorMessage(ModelState);
                    ModelState.AddModelError("", modelErrorMessage);

                    // set alert
                    ShowAlertBox(AlertType.Error, modelErrorMessage);

                    return View(collection);
                }

                // check if password and confirmPassword match
                if (!collection.Password.Equals(collection.ConfirmPassword))
                {
                    string errorMessage = "The password and confirmation password do not match";
                    ModelState.AddModelError("", errorMessage);

                    // set alert
                    ShowAlertBox(AlertType.Error, errorMessage);

                    return View(collection);
                }

                // check if loginId exists
                List<Login> listDuplicateLogins = loginService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.LoginName.Equals(collection.LoginName)).ToList();

                if (listDuplicateLogins.Count.Equals(0))
                {
                    string userUUId = Utility.Hypenate2124(Utility.GetUUID(21));
                    string sessonUUId = Utility.Hypenate2124(Utility.GetUUID(21));
                    string loginUUId = Utility.Hypenate2124(Utility.GetUUID(21));

                    Sesson sesson = new()
                    {
                        SessonUUId = sessonUUId,
                        SToken = Guid.NewGuid().ToString(),
                        LastAccessTime = DateTime.Now
                    };

                    Login login = new()
                    {
                        LoginUUId = loginUUId,
                        LoginName = collection.LoginName,
                        LoginType = LoginCategory.Username,
                        LastLoginDate = DateTime.Now,
                        LToken = Guid.NewGuid().ToString(),
                        // set generic data
                        StatusFlag = RecordStatus.Active.ToUpper(),
                        ModifiedBy = 1,
                        ModifiedDate = DateTime.Now
                    };

                    User user = new()
                    {
                        // set core data
                        FirstName = collection.FirstName,
                        LastName = collection.LastName,
                        Email = collection.Email,
                        Mobile = collection.CellPhone,
                        UserUUId = userUUId,
                        SessonUUId = sessonUUId,
                        LoginUUId = loginUUId,
                        // set generic data
                        StatusFlag = RecordStatus.Active.ToUpper(),
                        ModifiedBy = 1,
                        ModifiedDate = DateTime.Now
                    };

                    // hash the password
                    PasswordHasher<Login> passwordHasher = new PasswordHasher<Login>();
                    string hashedPassword = passwordHasher.HashPassword(login, collection.Password);

                    login.PasswordHash = hashedPassword;

                    using (TransactionScope scope = new TransactionScope())
                    {
                        sessonService.InsertRecord(sesson);
                        sessonService.SaveDBChanges();

                        loginService.InsertRecord(login);
                        loginService.SaveDBChanges();

                        userService.InsertRecord(user);
                        userService.SaveDBChanges();

                        scope.Complete();
                    }

                    // set alert
                    ShowAlertBox(AlertType.Success, "Registration is successfully complete.");

                    return RedirectToAction(nameof(Login), new { controller = "Account" });
                }
                else
                {
                    // set alert box
                    ShowAlertBox(AlertType.Warning, "Login Id is already taken. Please try another name.");

                    return View(collection);
                }
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "AccountController.293 " + errorMessage);

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Register));
            }
        }

        // GET: /Identity/Account/ChangePassword
        [AllowAnonymous]
        //[Permit(Permits.Identity.Login.ChangePassword)]
        public IActionResult ChangePassword()
        {
            ChangePasswordViewModel model = new();

            return View(model);
        }

        // POST: /Identity/Account/ChangePassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //[Permit(Permits.Identity.Login.ChangePassword)]
        public IActionResult ChangePassword(ChangePasswordViewModel collection)
        {
            bool passwordIsVerified = false;

            try
            {
                // return if model is invalid
                if (!ModelState.IsValid)
                {
                    string modelErrorMessage = GetModelErrorMessage(ModelState);
                    ModelState.AddModelError("", modelErrorMessage);

                    // set alert
                    ShowAlertBox(AlertType.Error, modelErrorMessage);

                    return View(collection);
                }

                // check if password and confirmPassword match
                if (!collection.NewPassword.Equals(collection.ConfirmPassword))
                {
                    string errorMessage = "The password and confirmation password do not match";
                    ModelState.AddModelError("", errorMessage);

                    // set alert
                    ShowAlertBox(AlertType.Error, errorMessage);

                    return View(collection);
                }

                if (currUser.UserId != collection.UserId)
                {
                    string errorMessage = "Invalid user";
                    ModelState.AddModelError("", errorMessage);

                    // set alert
                    ShowAlertBox(AlertType.Error, errorMessage);

                    return View(collection);
                }

                Login login = loginService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())
                                                                && c.LoginUUId.Equals(currUser.LoginUUId)).SingleOrDefault();

                // verify old password
                try
                {
                    //verify password
                    PasswordVerificationResult result = new PasswordHasher<Login>().VerifyHashedPassword(login, login.PasswordHash, collection.OldPassword);

                    if (result == PasswordVerificationResult.Success) passwordIsVerified = true;
                    //else if (result == PasswordVerificationResult.SuccessRehashNeeded) passwordIsVerified = false;
                    //else if (result == PasswordVerificationResult.Failed) passwordIsVerified = false;
                }
                catch (Exception ex)
                {
                    // prepare data
                    string errorMessage = GetInnerExceptionMessage(ex);

                    // log
                    logger.Log(LogLevel.Error, ex, "AccountController.411" + errorMessage);
                }

                if (!passwordIsVerified)
                {
                    // set alert
                    ShowAlertBox(AlertType.Warning, "Old password match has failed. Try again.");

                    return RedirectToAction("ChangePassword", "Account", new { area = "Identity" });
                }

                // hash the password
                PasswordHasher<Login> passwordHasher = new PasswordHasher<Login>();
                string hashedPassword = passwordHasher.HashPassword(login, collection.NewPassword);

                login.PasswordHash = hashedPassword;
                // set generic data
                login.ModifiedBy = currUser.UserId;
                login.ModifiedDate = DateTime.Now;

                loginService.SaveDBChanges();

                // set alert
                ShowAlertBox(AlertType.Success, "Password is successfully changed.");

                return RedirectToAction(nameof(Login), new { controller = "Account" });

            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "AccountController.384");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                //return RedirectToAction(nameof(Register));
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        // POST: /Identity/Account/LogOff
        public IActionResult LogOff()
        {
            if (currUser.UserId > 0)
            {
                // scramble SToken
                Sesson sesson = sessonService.GetRecordsByFilter(c => c.SessonUUId.Equals(currUser.SessonUUId)).SingleOrDefault();

                sesson.SToken = Guid.NewGuid().ToString();
                sesson.LastAccessTime = DateTime.Now.AddMinutes(-60);

                sessonService.UpdateRecord(sesson);
                sessonService.SaveDBChanges();

                // scramble RToken
                Login login = loginService.GetRecordsByFilter(c => c.LoginUUId.Equals(currUser.LoginUUId)).SingleOrDefault();

                login.LToken = Guid.NewGuid().ToString();
                // set generic data
                login.ModifiedBy = currUser.UserId;
                login.ModifiedDate = DateTime.Now;

                loginService.UpdateRecord(login);
                loginService.SaveDBChanges();

                //note: no need for Transaction Scoping the above transaction changes, even if one passes we are good
            }

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (currUser.UserId > 0)
            {
                // set alert
                ShowAlertBox(AlertType.Info, "User is successfully logged off.");
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}