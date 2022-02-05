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
        private readonly IPermitService permitService;

        public MenuController(ILogger<MenuController> logger, ICoreService coreService, IPermitService permitService) : base(logger, coreService)
		{
            this.cacheService = coreService.GetCacheService();
            this.cacheKeyService = coreService.GetCacheKeyService();
            this.userService = coreService.GetUserService();
            this.menuService = coreService.GetMenuService();
            this.menuPermitService = coreService.GetMenuPermitService();
            this.permitService = permitService;
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
			var menuList = coreService.GetMenuHierarchy(RecordStatus.Active, "URL");

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
            var parentMenu = menuService.GetRecordById(id);
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
                    MenuName = collection.MenuName.Trim(),
                    MenuDescription = collection.MenuDescription.Trim(),
                    MenuURL = collection.MenuURL.Trim(),
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

                // update treePath and hLevel
                UpdateTreePath();

                // reset cache
                ResetCache();

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
            var model = (MenuViewModel)menuService.GetRecordById(id);
            var modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // GET: /Identity/Menu/Update/5
        [Permit(Permits.Identity.Menu.Update)]
        public ActionResult Update(int id)
        {
            var model = (MenuMiniViewModel)menuService.GetRecordById(id);

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
                var recordMenu = menuService.GetRecordById(id);

                if (recordMenu.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()))
                {
                    // set core data
                    recordMenu.MenuName = collection.MenuName.Trim();
                    recordMenu.MenuDescription = collection.MenuDescription.Trim();
                    recordMenu.MenuURL = collection.MenuURL.Trim();
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

                // update treePath and hLevel
                UpdateTreePath();

                // reset cache
                ResetCache();

                // set alert
                ShowAlertBox(AlertType.Success, "Menu is successfully updated.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "MenuController.217");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Update), new { id });
            }
        }

        // GET: /Identity/Menu/Delete/5
        [Permit(Permits.Identity.Menu.Delete)]
        public ActionResult Delete(int id)
        {
            var model = (MenuViewModel)menuService.GetRecordById(id);
            var modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

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
                var recordMenu = menuService.GetRecordById(id);

                if (recordMenu.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()))
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

                // update treePath and hLevel
                UpdateTreePath();

                // reset cache
                ResetCache();

                // set alert
                ShowAlertBox(AlertType.Success, "Menu is successfully deleted.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "MenuController.293");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // POST: /Identity/Menu/ListDeleted
        [Permit(Permits.Identity.Menu.ListDeleted)]
        public ActionResult ListDeleted(string temptemp)
        {
            var menuList = coreService.GetMenuService().GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Inactive.ToUpper())).ToList();

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
            var model = (MenuViewModel)menuService.GetRecordById(id);
            var modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

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
                var recordMenu = menuService.GetRecordById(id);

                if (recordMenu.StatusFlag.ToUpper().Equals(RecordStatus.Inactive.ToUpper()))
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

                // update treePath and hLevel
                UpdateTreePath();

                // reset cache
                ResetCache();

                // set alert
                ShowAlertBox(AlertType.Success, "Menu is successfully restored.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "MenuController.385");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET: /Identity/Menu/Permit/5
        [Permit(Permits.Identity.Menu.MenuPermits)]
        public ActionResult MenuPermits(int id)
        {
            if (id <= 0)
            {
                // set alert
                ShowAlertBox(AlertType.Error, "Menu Id is not valid.");

                return RedirectToAction(nameof(Index));
            }

            var menu = menuService.GetRecordById(id);
            var listPermits = permitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).OrderBy(c => c.PermitArea).ThenBy(c => c.PermitController).ThenBy(c => c.PermitName).ToList();
            var listMenuPermitIds = menuPermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.MenuId.Equals(id)).OrderBy(c => c.PermitId).Select(c => c.PermitId).ToList();
            var listAreas = listPermits.OrderBy(c => c.PermitArea).Select(c => c.PermitArea).Distinct().ToList();

            // set ViewBag
            ViewBag.menu = menu;
            ViewBag.listAreas = listAreas;
            ViewBag.listPermits = listPermits;
            ViewBag.listMenuPermitIds = listMenuPermitIds;

            return View();
        }

        // GET: /Identity/Menu/Permit/5
        [HttpPost]
        [Permit(Permits.Identity.Menu.MenuPermits)]
        public ActionResult MenuPermits(int id, List<int> permitIds)
        {
			var listMenuPermits = menuPermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.MenuId.Equals(id)).ToList();

			// delete all permits for the menu
			foreach (var menuPermit in listMenuPermits)
			{
				menuPermit.StatusFlag = RecordStatus.Inactive;
				menuPermit.ModifiedBy = currUserMeta.UserId;
				menuPermit.ModifiedDate = DateTime.Now;
			}

			menuPermitService.SaveDBChanges();

			// add or activate received permits for the menu
			foreach (var permitID in permitIds)
			{
				var existingMenuPermit = menuPermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.MenuId.Equals(id) && c.PermitId.Equals(permitID)).FirstOrDefault();

				if (existingMenuPermit != null)
				{
					existingMenuPermit.StatusFlag = RecordStatus.Active;
					existingMenuPermit.ModifiedBy = currUserMeta.UserId;
					existingMenuPermit.ModifiedDate = DateTime.Now;

					menuPermitService.UpdateRecord(existingMenuPermit);
				}
				else
				{
					MenuPermit newMenuPermit = new()
					{
						MenuId = id,
						PermitId = permitID,
						StatusFlag = RecordStatus.Active,
						ModifiedBy = 1,
						ModifiedDate = DateTime.Now
					};

					menuPermitService.InsertRecord(newMenuPermit);
				}
			}

			menuPermitService.SaveDBChanges();

            // update treePath and hLevel
            UpdateTreePath();

            // reset cache
            ResetCache();

            // set alert
            ShowAlertBox(AlertType.Success, "Menu Permits are successfully updated.");

            //return Permit(id);
            return RedirectToAction(nameof(Index));
        }
    
        private void UpdateTreePath()
		{
            List<Menu> menuHierarchy = menuService.GetMenuHierarchy(RecordStatus.Active, "URL");

            foreach (var itemMenu in menuHierarchy)
			{
                var recordMenu = menuService.GetRecordById(itemMenu.MenuId);

                recordMenu.HLevel = itemMenu.HLevel;
                recordMenu.TreePath = itemMenu.TreePath;

                menuService.UpdateRecord(recordMenu);
            }

            menuService.SaveDBChanges();
        }

        private void ResetCache()
        {
            //// cache :: remove following :: 
            //// ALL
            var listCacheKeyNames = cacheKeyService.GetRecordsByFilter(c => c.ModifiedDate >= DateTime.Now.AddMinutes(-240)).OrderBy(c => c.CacheKeyName).Select(c => c.CacheKeyName).ToList();

            foreach (var itemCacheKeyName in listCacheKeyNames)
            {
                cacheService.RemoveCache(itemCacheKeyName.ToString());
            }
        }
    }
}
