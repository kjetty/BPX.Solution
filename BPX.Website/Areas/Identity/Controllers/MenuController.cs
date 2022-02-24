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
            List<Menu> listMenus = coreService.GetMenuHierarchy(RecordStatus.Active.ToUpper(), "URL");
            List<MenuMiniViewModel> model = new List<MenuMiniViewModel>();

            foreach (Menu itemMenu in listMenus)
            {
                model.Add((MenuMiniViewModel)itemMenu);
            }

            return View(model);
		}

        // GET: /Identity/Menu/Create
        [Permit(Permits.Identity.Menu.Create)]
        public IActionResult Create(int id)
        {
            //get parentMenu detail
            Menu parentMenu = menuService.GetRecordById(id);
            ViewBag.parentMenu = parentMenu;

            return View();
        }

        // POST: /Identity/Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Menu.Create)]        
        public IActionResult Create(MenuMiniViewModel collection)
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
                    StatusFlag = RecordStatus.Active.ToUpper(),
                    ModifiedBy = currUser.UserId,
                    ModifiedDate = DateTime.Now
                };

                // add record
                menuService.InsertRecord(recordMenu);

                // commit changes to database
                menuService.SaveDBChanges();

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
        public IActionResult Read(int id)
        {
            MenuViewModel model = (MenuViewModel)menuService.GetRecordById(id);
            User modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // GET: /Identity/Menu/Update/5
        [Permit(Permits.Identity.Menu.Update)]
        public IActionResult Update(int id)
        {
            MenuMiniViewModel model = (MenuMiniViewModel)menuService.GetRecordById(id);

            return View(model);
        }

        // POST: /Identity/Menu/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Menu.Update)]
        public IActionResult Update(int id, MenuMiniViewModel collection)
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
                Menu recordMenu = menuService.GetRecordById(id);

                if (recordMenu.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()))
                {
                    // set core data
                    recordMenu.MenuName = collection.MenuName;
                    recordMenu.MenuDescription = collection.MenuDescription;
                    recordMenu.MenuURL = collection.MenuURL;
                    recordMenu.ParentMenuId = collection.ParentMenuId;
                    recordMenu.OrderNumber = collection.OrderNumber;
                    // set generic data
                    recordMenu.StatusFlag = RecordStatus.Active.ToUpper();
                    recordMenu.ModifiedBy = currUser.UserId;
                    recordMenu.ModifiedDate = DateTime.Now;

                    // edit record
                    menuService.UpdateRecord(recordMenu);
                }

                // commit changes to database
                menuService.SaveDBChanges();

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
        public IActionResult Delete(int id)
        {
            MenuViewModel model = (MenuViewModel)menuService.GetRecordById(id);
            User modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Menu/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Menu.Delete)]
        public IActionResult Delete(int id, IFormCollection collection)
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
                Menu recordMenu = menuService.GetRecordById(id);

                if (recordMenu.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()))
                {
                    // set generic data
                    recordMenu.StatusFlag = RecordStatus.Inactive.ToUpper();
                    recordMenu.ModifiedBy = currUser.UserId;
                    recordMenu.ModifiedDate = DateTime.Now;

                    // edit record
                    menuService.UpdateRecord(recordMenu);
                }

                // commit changes to database
                menuService.SaveDBChanges();

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
        public IActionResult ListDeleted()
        {
            List<Menu> listMenus = coreService.GetMenuService().GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Inactive.ToUpper())).ToList();
            List<MenuMiniViewModel> model = new List<MenuMiniViewModel>();

            foreach (Menu itemMenu in listMenus)
            {
                model.Add((MenuMiniViewModel)itemMenu);
            }

            return View(model);
        }

        // GET: /Identity/Menu/Undelete/5
        [Permit(Permits.Identity.Menu.Undelete)]
        public IActionResult Undelete(int id)
        {
            MenuViewModel model = (MenuViewModel)menuService.GetRecordById(id);
            User modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Menu/Undelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Menu.Undelete)]
        public IActionResult Undelete(int id, IFormCollection collection)
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
                Menu recordMenu = menuService.GetRecordById(id);

                if (recordMenu.StatusFlag.ToUpper().Equals(RecordStatus.Inactive.ToUpper()))
                {
                    // set generic data
                    recordMenu.StatusFlag = RecordStatus.Active.ToUpper();
                    recordMenu.ModifiedBy = currUser.UserId;
                    recordMenu.ModifiedDate = DateTime.Now;

                    // edit record
                    menuService.UpdateRecord(recordMenu);
                }

                // commit changes to database
                menuService.SaveDBChanges();                

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
        public IActionResult MenuPermits(int id)
        {
            if (id <= 0)
            {
                // set alert
                ShowAlertBox(AlertType.Error, "Menu Id is not valid.");

                return RedirectToAction(nameof(Index));
            }

            Menu menu = menuService.GetRecordById(id);
            List<Permit> listPermits = permitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).OrderBy(c => c.PermitArea).ThenBy(c => c.PermitController).ThenBy(c => c.PermitName).ToList();
            List<int> listMenuPermitIds = menuPermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.MenuId.Equals(id)).OrderBy(c => c.PermitId).Select(c => c.PermitId).ToList();
            List<string> listAreas = listPermits.OrderBy(c => c.PermitArea).Select(c => c.PermitArea).Distinct().ToList();

            // set ViewBag
            ViewBag.menu = menu;
            ViewBag.listPermits = listPermits;
            ViewBag.listMenuPermitIds = listMenuPermitIds;
            ViewBag.listAreas = listAreas;
            
            return View();
        }

        // GET: /Identity/Menu/Permit/5
        [HttpPost]
        [Permit(Permits.Identity.Menu.MenuPermits)]
        public IActionResult MenuPermits(int id, List<int> permitIds)
        {
            // get all exisiting active permits for the menu
			List<MenuPermit> listMenuPermits = menuPermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.MenuId.Equals(id)).ToList();

            // delete all existing active permits for the menu
            foreach (MenuPermit menuPermit in listMenuPermits)
			{
				menuPermit.StatusFlag = RecordStatus.Inactive.ToUpper();
				menuPermit.ModifiedBy = currUser.UserId;
				menuPermit.ModifiedDate = DateTime.Now;

                menuPermitService.UpdateRecord(menuPermit);
            }           

            menuPermitService.SaveDBChanges();

			// add or activate received permits for the menu
			foreach (int permitId in permitIds)
			{
				MenuPermit existingMenuPermit = menuPermitService.GetRecordsByFilter(c => c.MenuId.Equals(id) && c.PermitId.Equals(permitId)).SingleOrDefault();

				if (existingMenuPermit != null)
				{
					existingMenuPermit.StatusFlag = RecordStatus.Active.ToUpper();
					existingMenuPermit.ModifiedBy = currUser.UserId;
					existingMenuPermit.ModifiedDate = DateTime.Now;

					menuPermitService.UpdateRecord(existingMenuPermit);
				}
				else
				{
					MenuPermit newMenuPermit = new()
					{
						MenuId = id,
						PermitId = permitId,
						StatusFlag = RecordStatus.Active.ToUpper(),
						ModifiedBy = currUser.UserId,
						ModifiedDate = DateTime.Now
					};

					menuPermitService.InsertRecord(newMenuPermit);
				}
			}

			menuPermitService.SaveDBChanges();          

            // reset cache
            ResetCache();

            // set alert
            ShowAlertBox(AlertType.Success, "Menu Permits are successfully updated.");

            //return Permit(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Identity/Menu/TreePath
        [Permit(Permits.Identity.Menu.TreePath)]
        public IActionResult TreePath()
		{
            List<Menu> listMenus = menuService.GetMenuHierarchy(RecordStatus.Active.ToUpper(), "URL");

            foreach (Menu itemMenu in listMenus)
			{
                Menu recordMenu = menuService.GetRecordById(itemMenu.MenuId);

                recordMenu.HLevel = itemMenu.HLevel;
                recordMenu.TreePath = itemMenu.TreePath;

                menuService.UpdateRecord(recordMenu);
            }

            menuService.SaveDBChanges();

            // set alert
            ShowAlertBox(AlertType.Success, "Menu TreePath is successfully updated.");

            return RedirectToAction(nameof(List));
        }

        private void ResetCache()
        {
            //// cache :: remove following :: 
            //// ALL
            List<string> listCacheKeyNames = cacheKeyService.GetRecordsByFilter(c => c.ModifiedDate >= DateTime.Now.AddDays(-999)).OrderBy(c => c.CacheKeyName).Select(c => c.CacheKeyName).ToList();

            foreach (string itemCacheKeyName in listCacheKeyNames)
            {
                cacheService.RemoveCache(itemCacheKeyName.ToString());
            }
        }
    }
}
