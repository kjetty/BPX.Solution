using BPX.Domain.DbModels;
using BPX.Domain.ViewModels;
using BPX.Service;
using BPX.Utils;
using BPX.Website.Controllers;
using BPX.Website.CustomCode.Authorize;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPX.Website.Areas.Identity.Controllers
{
	[Area("Identity")]
	public class MenuController : BaseController<MenuController>
	{
        private readonly ICacheService cacheService;
        private readonly ICacheKeyService cacheKeyService;
        private readonly IUserService userService;
        private readonly IMenuService menuService;
        private readonly IMenuPermitService menuPermitService;

        public MenuController(ILogger<MenuController> logger, ICoreService coreService) : base(logger, coreService)
		{
            this.cacheService = coreService.GetCacheService();
            this.cacheKeyService = coreService.GetCacheKeyService();
            this.userService = coreService.GetUserService();
            this.menuService = coreService.GetMenuService();
            this.menuPermitService = coreService.GetMenuPermitService();
        }

        // GET: /Identity/Menu
        [Permit(Permits.Identity.Menu.List)]
        public IActionResult Index()
		{
			return RedirectToAction("List");
		}

		// GET: /Identity/Menu/List
		[Permit(Permits.Identity.Menu.List)]
		public IActionResult List()
		{
			var menuList = coreService.GetMenuHierarchy(RecordStatus.Active, "url");

			List<MenuMiniViewModel> model = new List<MenuMiniViewModel>();

			foreach (var itemMenu in menuList)
			{
				model.Add((MenuMiniViewModel)itemMenu);
			}

			return View(model);
		}

        // GET: /Identity/Menu/Create
        [Permit(Permits.Identity.Menu.Create)]
        public ActionResult Create(int id)
        {
            //get parentMenu detail
            var parentMenu = menuService.GetRecordByID(id);
            ViewBag.parentMenu = parentMenu;

            return View();
        }

        // POST: /Identity/Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Menu.Create)]        
        public ActionResult Create(MenuMiniViewModel collection)
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

                Menu recordMenu = new()
                {
                    // set core data
                    MenuName = collection.MenuName,
                    MenuDescription = collection.MenuDescription,
                    MenuURL = collection.MenuURL,
                    ParentMenuId = collection.ParentMenuId,
                    OrderNumber = collection.OrderNumber,
                    // set generic data
                    StatusFlag = RecordStatus.Active,
                    ModifiedBy = 1,
                    ModifiedDate = DateTime.Now
                };

                // add record
                menuService.InsertRecord(recordMenu);

                // commit changes to database
                menuService.SaveDBChanges();

                // handle cache :: remove all cache related to :: menu
                var cacheKeys = cacheKeyService.GetRecordsByFilter(c => c.CacheKeyName.Contains("menu")).OrderBy(c => c.CacheKeyName).ToList();
                foreach (var itemCacheKey in cacheKeys)
                {
                    cacheService.RemoveCache(itemCacheKey.CacheKeyName);
                }

                // set alert
                ShowAlertBox(AlertType.Success, "Menu is successfully created.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "MenuController.136");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);
                return RedirectToAction(nameof(Create));
            }
        }

        // GET: /Identity/Menu/Read/5
        [Permit(Permits.Identity.Menu.Read)]
        public ActionResult Read(int id)
        {
            var model = (MenuViewModel)menuService.GetRecordByID(id);
            var modelModifiedBy = userService.GetRecordByID(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // GET: /Identity/Menu/Update/5
        [Permit(Permits.Identity.Menu.Update)]
        public ActionResult Update(int id)
        {
            var model = (MenuMiniViewModel)menuService.GetRecordByID(id);

            return View(model);
        }

        // POST: /Identity/Menu/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Menu.Update)]
        public ActionResult Update(int id, MenuMiniViewModel collection)
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

                // get existing data
                var recordMenu = menuService.GetRecordByID(id);

                if (recordMenu.StatusFlag == RecordStatus.Active)
                {
                    // set core data
                    recordMenu.MenuName = collection.MenuName;
                    recordMenu.MenuDescription = collection.MenuDescription;
                    recordMenu.MenuURL = collection.MenuURL;
                    recordMenu.ParentMenuId = collection.ParentMenuId;
                    recordMenu.OrderNumber = collection.OrderNumber;
                    // set generic data
                    recordMenu.StatusFlag = RecordStatus.Active;
                    recordMenu.ModifiedBy = currUserMeta.UserId;
                    recordMenu.ModifiedDate = DateTime.Now;

                    // edit record
                    menuService.UpdateRecord(recordMenu);
                }

                // commit changes to database
                menuService.SaveDBChanges();

                // handle cache :: remove all cache related to :: menu
                var cacheKeys = cacheKeyService.GetRecordsByFilter(c => c.CacheKeyName.Contains("menu")).OrderBy(c => c.CacheKeyName).ToList();
                foreach (var itemCacheKey in cacheKeys)
                {
                    cacheService.RemoveCache(itemCacheKey.CacheKeyName);
                }

                // set alert
                ShowAlertBox(AlertType.Success, "Menu is successfully updated.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "MenuController.204");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Update), new { id });
            }
        }

        // GET: /Identity/Menu/Delete/5
        [Permit(Permits.Identity.Menu.Delete)]
        public ActionResult Delete(int id)
        {
            var model = (MenuViewModel)menuService.GetRecordByID(id);
            var modelModifiedBy = userService.GetRecordByID(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Menu/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Menu.Delete)]
        public ActionResult Delete(int id, IFormCollection collection)
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

                // get existing data
                var recordMenu = menuService.GetRecordByID(id);

                if (recordMenu.StatusFlag == RecordStatus.Active)
                {
                    // set generic data
                    recordMenu.StatusFlag = RecordStatus.Inactive;
                    recordMenu.ModifiedBy = currUserMeta.UserId;
                    recordMenu.ModifiedDate = DateTime.Now;

                    // edit record
                    menuService.UpdateRecord(recordMenu);
                }

                // commit changes to database
                menuService.SaveDBChanges();

                // handle cache :: remove all cache related to :: menu
                var cacheKeys = cacheKeyService.GetRecordsByFilter(c => c.CacheKeyName.Contains("menu")).OrderBy(c => c.CacheKeyName).ToList();
                foreach (var itemCacheKey in cacheKeys)
                {
                    cacheService.RemoveCache(itemCacheKey.CacheKeyName);
                }

                // set alert
                ShowAlertBox(AlertType.Success, "Menu is successfully deleted.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "MenuController.274");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // POST: /Identity/Menu/ListDeleted
        [Permit(Permits.Identity.Menu.ListDeleted)]
        public ActionResult ListDeleted(string temptemp)
        {
            var menuList = coreService.GetMenuService().GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Inactive)).ToList();

            List<MenuMiniViewModel> model = new List<MenuMiniViewModel>();

            foreach (var itemMenu in menuList)
            {
                model.Add((MenuMiniViewModel)itemMenu);
            }

            return View(model);
        }

        // GET: /Identity/Menu/Undelete/5
        [Permit(Permits.Identity.Menu.Undelete)]
        public ActionResult Undelete(int id)
        {
            var model = (MenuViewModel)menuService.GetRecordByID(id);
            var modelModifiedBy = userService.GetRecordByID(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Menu/Undelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Menu.Undelete)]
        public ActionResult Undelete(int id, IFormCollection collection)
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

                // get existing data
                var recordMenu = menuService.GetRecordByID(id);

                if (recordMenu.StatusFlag == RecordStatus.Inactive)
                {
                    // set generic data
                    recordMenu.StatusFlag = RecordStatus.Active;
                    recordMenu.ModifiedBy = currUserMeta.UserId;
                    recordMenu.ModifiedDate = DateTime.Now;

                    // edit record
                    menuService.UpdateRecord(recordMenu);
                }

                // commit changes to database
                menuService.SaveDBChanges();

                // handle cache :: remove all cache related to :: menu
                var cacheKeys = cacheKeyService.GetRecordsByFilter(c => c.CacheKeyName.Contains("menu")).OrderBy(c => c.CacheKeyName).ToList();
                foreach (var itemCacheKey in cacheKeys)
                {
                    cacheService.RemoveCache(itemCacheKey.CacheKeyName);
                }

                // set alert
                ShowAlertBox(AlertType.Success, "Menu is successfully restored.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "MenuController.378");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET: /Identity/Menu/Permit/5
        [Permit(Permits.Identity.MenuPermit.CRUD)]
        public ActionResult Permit(int id)
        {
            if (id <= 0)
            {
                // set alert
                ShowAlertBox(AlertType.Error, "Menu Id is not valid.");

                return RedirectToAction(nameof(Index));
            }

            var menu = menuService.GetRecordByID(id);
            //var listPermits = permitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active)).OrderBy(c => c.PermitArea).ThenBy(c => c.PermitController).ThenBy(c => c.PermitName).ToList();
            //var listMenuPermitIDs = menuPermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.MenuId.Equals(id)).OrderBy(c => c.PermitID).Select(c => c.PermitID).ToList();
            //var listAreas = listPermits.OrderBy(c => c.PermitArea).Select(c => c.PermitArea).Distinct().ToList();

            //// set ViewBag
            //ViewBag.menu = menu;
            //ViewBag.listAreas = listAreas;
            //ViewBag.listPermits = listPermits;
            //ViewBag.listMenuPermitIDs = listMenuPermitIDs;

            return View();
        }

        // GET: /Identity/Menu/Permit/5
        [HttpPost]
        [Permit(Permits.Identity.MenuPermit.CRUD)]
        public ActionResult Permit(int id, List<int> permitIDs)
        {
            //var listMenuPermits = menuPermitService.GetRecordsByFilter(c => c.MenuId == id).ToList();

            //// delete all permits for the menu
            //foreach (var menuPermit in listMenuPermits)
            //{
            //    menuPermit.StatusFlag = RecordStatus.Inactive;
            //    menuPermit.ModifiedBy = currUserMeta.UserId;
            //    menuPermit.ModifiedDate = DateTime.Now;
            //}

            //menuPermitService.SaveDBChanges();

            //// add or activate received permits for the menu
            //foreach (var permitID in permitIDs)
            //{
            //    var existingMenuPermit = menuPermitService.GetRecordsByFilter(c => c.MenuId == id && c.PermitID == permitID).FirstOrDefault();

            //    if (existingMenuPermit != null)
            //    {
            //        existingMenuPermit.StatusFlag = RecordStatus.Active;
            //        existingMenuPermit.ModifiedBy = currUserMeta.UserId;
            //        existingMenuPermit.ModifiedDate = DateTime.Now;

            //        menuPermitService.UpdateRecord(existingMenuPermit);
            //    }
            //    else
            //    {
            //        MenuPermit newMenuPermit = new()
            //        {
            //            MenuId = id,
            //            PermitID = permitID,
            //            StatusFlag = RecordStatus.Active,
            //            ModifiedBy = 1,
            //            ModifiedDate = DateTime.Now
            //        };

            //        menuPermitService.InsertRecord(newMenuPermit);
            //    }
            //}

            //menuPermitService.SaveDBChanges();

            //// remove from cache
            //string cacheKey = string.Empty;

            //// user:{id}:menus
            //cacheKey = $"menu:{id}:permits";
            //cacheService.RemoveCache(cacheKey);

            //// user:{id}:meta
            ////cacheKey = $"user:{id}:meta";
            ////cacheService.RemoveCache(cacheKey);

            //// permit:{id}:menus
            //var cacheKeys = cacheKeyService.GetRecordsByFilter(c => c.CacheKeyName.StartsWith("permit:")).OrderBy(c => c.CacheKeyName).ToList();
            //foreach (var itemCacheKey in cacheKeys)
            //{
            //    cacheService.RemoveCache(itemCacheKey.CacheKeyName);
            //}


            // set alert
            ShowAlertBox(AlertType.Success, "Menu Permits are successfully updated.");

            //return Permit(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
