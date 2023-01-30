using BPX.Domain.DbModels;
using BPX.Domain.ViewModels;
using BPX.Service;
using BPX.Utils;
using BPX.Website.Controllers;
using BPX.Website.Filters.Authorize;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using X.PagedList;

namespace BPX.Website.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class PermitController : BaseController<PermitController>
    {
        private readonly ICacheService cacheService;
        private readonly ICacheKeyService cacheKeyService;
        private readonly IUserService userService;
        private readonly IMenuService menuService;
        private readonly IPermitService permitService;
        private readonly IRoleService roleService;
        private readonly IRolePermitService rolePermitService;
        private readonly IMenuPermitService menuPermitService;

        public PermitController(ILogger<PermitController> logger, ICoreService coreService, IPermitService permitService, IRoleService roleService) : base(logger, coreService)
        {
            this.cacheService = coreService.GetCacheService();
            this.cacheKeyService = coreService.GetCacheKeyService();
            this.userService = coreService.GetUserService();
            this.menuService = coreService.GetMenuService();
            this.permitService = permitService;
            this.roleService = roleService;
            this.rolePermitService = coreService.GetRolePermitService();
            this.menuPermitService = coreService.GetMenuPermitService();
        }

        // GET: /Identity/Permit
        [Permit(Permits.Identity.Permit.List)]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // Get: /Identity/Permit/List
        [Permit(Permits.Identity.Permit.List)]
        public IActionResult List(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            // check input and set defaults
            pageNumber = (pageNumber <= 0) ? 1 : pageNumber;
            pageSize = (pageSize <= 0) ? bpxPageSize : pageSize;
            statusFlag = RecordStatus.Active.ToUpper();   //force set to Active records always
            sortByColumn = (sortByColumn == null || sortByColumn.Trim().Length.Equals(0)) ? string.Empty : sortByColumn;
            sortOrder = (sortOrder == null || sortOrder.Trim().Length.Equals(0)) ? SortOrder.Ascending.ToUpper() : sortOrder;
            searchForString = (searchForString == null || searchForString.Trim().Length.Equals(0)) ? string.Empty : searchForString;
            filterJson = (filterJson == null || filterJson.Trim().Length.Equals(0)) ? string.Empty : filterJson;

            // fetch data
            IPagedList<PermitMiniViewModel> model = permitService.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString, filterJson).Select(c => (PermitMiniViewModel)c);

            // set pagination data
            ViewBag.pageNumber = pageNumber;
            ViewBag.pageSize = pageSize;
            ViewBag.statusFlag = statusFlag;
            ViewBag.sortByColumn = sortByColumn;
            ViewBag.sortOrder = sortOrder;
            ViewBag.searchForString = searchForString;
            ViewBag.filterJson = filterJson;

            return View(model);
        }

        // GET: /Identity/Permit/Create
        [Permit(Permits.Identity.Permit.Create)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Identity/Permit/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Permit.Create)]
        public IActionResult Create(PermitMiniViewModel collection)
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

                Permit recordPermit = new()
                {
                    // set core data
                    PermitArea = collection.PermitArea,
                    PermitController = collection.PermitController,
                    PermitName = collection.PermitName,
                    PermitEnum = collection.PermitEnum,
                    // set generic data
                    StatusFlag = RecordStatus.Active.ToUpper(),
                    ModifiedBy = currUser.UserId,
                    ModifiedDate = DateTime.Now
                };

                // add record
                permitService.InsertRecord(recordPermit);

                // commit changes to database
                permitService.SaveDBChanges();

                // set alert
                ShowAlertBox(AlertType.Success, "Permit is successfully created.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "PermitController.135");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Create));
            }
        }

        // GET: /Identity/Permit/Read/5
        [Permit(Permits.Identity.Permit.Read)]
        public IActionResult Read(int id)
        {
            PermitViewModel model = (PermitViewModel)permitService.GetRecordById(id);
            User modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // GET: /Identity/Permit/Update/5
        [Permit(Permits.Identity.Permit.Update)]
        public IActionResult Update(int id)
        {
            PermitMiniViewModel model = (PermitMiniViewModel)permitService.GetRecordById(id);

            return View(model);
        }

        // POST: /Identity/Permit/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Permit.Update)]
        public IActionResult Update(int id, PermitMiniViewModel collection)
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
                Permit recordPermit = permitService.GetRecordById(id);

                if (recordPermit.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()))
                {
                    // set core data
                    recordPermit.PermitArea = collection.PermitArea;
                    recordPermit.PermitController = collection.PermitController;
                    recordPermit.PermitName = collection.PermitName;
                    recordPermit.PermitEnum = collection.PermitEnum;
                    // set generic data
                    recordPermit.StatusFlag = RecordStatus.Active.ToUpper();
                    recordPermit.ModifiedBy = currUser.UserId;
                    recordPermit.ModifiedDate = DateTime.Now;

                    // edit record
                    permitService.UpdateRecord(recordPermit);
                }

                // commit changes to database
                permitService.SaveDBChanges();

                // reset cache
                ResetCache();

                // set alert
                ShowAlertBox(AlertType.Success, "Permit is successfully updated.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "PermitController.218");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Update), new { id });
            }
        }

        // GET: /Identity/Permit/Delete/5
        [Permit(Permits.Identity.Permit.Delete)]
        public IActionResult Delete(int id)
        {
            PermitViewModel model = (PermitViewModel)permitService.GetRecordById(id);
            User modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Permit/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Permit.Delete)]
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

                using (TransactionScope scope = new TransactionScope())
                {
                    // delete related menuPermits
                    List<MenuPermit> listMenuPermits = menuPermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.PermitId.Equals(id)).ToList();

                    foreach (MenuPermit itemMenuPermit in listMenuPermits)
                    {
                        itemMenuPermit.StatusFlag = RecordStatus.Inactive.ToUpper();
                        itemMenuPermit.ModifiedBy = currUser.UserId;
                        itemMenuPermit.ModifiedDate = DateTime.Now;

                        menuPermitService.UpdateRecord(itemMenuPermit);
                        menuPermitService.SaveDBChanges();
                    }

                    // delete related rolePermits
                    List<RolePermit> listRolePermits = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.PermitId.Equals(id)).ToList();

                    foreach (RolePermit itemRolePermit in listRolePermits)
                    {
                        itemRolePermit.StatusFlag = RecordStatus.Inactive.ToUpper();
                        itemRolePermit.ModifiedBy = currUser.UserId;
                        itemRolePermit.ModifiedDate = DateTime.Now;

                        rolePermitService.UpdateRecord(itemRolePermit);
                        rolePermitService.SaveDBChanges();
                    }

                    // delete permit
                    Permit recordPermit = permitService.GetRecordById(id);

                    recordPermit.StatusFlag = RecordStatus.Inactive.ToUpper();
                    recordPermit.ModifiedBy = currUser.UserId;
                    recordPermit.ModifiedDate = DateTime.Now;

                    permitService.UpdateRecord(recordPermit);
                    permitService.SaveDBChanges();

                    scope.Complete();
                }

                // reset cache
                ResetCache();

                // set alert
                ShowAlertBox(AlertType.Success, "Permit is successfully deleted.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "PermitController.291");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET + POST: /Identity/Permit/ListDeleted
        [Permit(Permits.Identity.Permit.ListDeleted)]
        public IActionResult ListDeleted(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            // check input and set defaults
            pageNumber = (pageNumber <= 0) ? 1 : pageNumber;
            pageSize = (pageSize <= 0) ? bpxPageSize : pageSize;
            statusFlag = RecordStatus.Inactive.ToUpper();   //force set to Active records always
            sortByColumn = (sortByColumn == null || sortByColumn.Trim().Length.Equals(0)) ? string.Empty : sortByColumn;
            sortOrder = (sortOrder == null || sortOrder.Trim().Length.Equals(0)) ? SortOrder.Ascending.ToUpper() : sortOrder;
            searchForString = (searchForString == null || searchForString.Trim().Length.Equals(0)) ? string.Empty : searchForString;
            filterJson = (filterJson == null || filterJson.Trim().Length.Equals(0)) ? string.Empty : filterJson;

            // fetch data
            IPagedList<PermitMiniViewModel> model = permitService.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString, filterJson).Select(c => (PermitMiniViewModel)c);

            // set pagination data
            ViewBag.pageNumber = pageNumber;
            ViewBag.pageSize = pageSize;
            ViewBag.statusFlag = statusFlag;
            ViewBag.sortByColumn = sortByColumn;
            ViewBag.sortOrder = sortOrder;
            ViewBag.searchForString = searchForString;
            ViewBag.filterJson = filterJson;

            return View(model);
        }

        // GET: /Identity/Permit/Undelete/5
        [Permit(Permits.Identity.Permit.Undelete)]
        public IActionResult Undelete(int id)
        {
            PermitViewModel model = (PermitViewModel)permitService.GetRecordById(id);
            User modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Permit/Undelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Permit.Undelete)]
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
                Permit recordPermit = permitService.GetRecordById(id);

                if (recordPermit.StatusFlag.Equals(RecordStatus.Inactive.ToUpper()))
                {
                    // set generic data
                    recordPermit.StatusFlag = RecordStatus.Active.ToUpper();
                    recordPermit.ModifiedBy = currUser.UserId;
                    recordPermit.ModifiedDate = DateTime.Now;

                    // edit record
                    permitService.UpdateRecord(recordPermit);
                }

                // commit changes to database
                permitService.SaveDBChanges();

                // reset cache
                ResetCache();

                // set alert
                ShowAlertBox(AlertType.Success, "Permit is successfully restored.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "PermitController.390");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET: /Identity/Permit/RolesAndMenus/5
        [Permit(Permits.Identity.Permit.Read)]
        public IActionResult PermitRolesMenus(int id)
        {
            if (id <= 0)
            {
                // set alert
                ShowAlertBox(AlertType.Error, "Permit Id is not valid.");

                return RedirectToAction(nameof(Index));
            }

            Permit permit = permitService.GetRecordById(id);
            List<int> listRoleIds = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.PermitId.Equals(id)).Select(c => c.RoleId).ToList();
            List<Role> listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && listRoleIds.Contains(c.RoleId)).OrderBy(c => c.RoleName).ToList();
            List<int> listMenuIds = menuPermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.PermitId.Equals(id)).Select(c => c.MenuId).ToList();
            List<Menu> listMenus = menuService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && listMenuIds.Contains(c.MenuId)).OrderBy(c => c.MenuURL).ToList();

            // set ViewBag
            ViewBag.permit = permit;
            ViewBag.listRoleIds = listRoleIds;
            ViewBag.listRoles = listRoles;
            ViewBag.listMenuIds = listMenuIds;
            ViewBag.listMenus = listMenus;

            return View();
        }

        private void ResetCache()
        {
            //// cache :: remove following :: 
            //// ALL
            List<string> listCacheKeyNames = cacheKeyService.GetRecordsByFilter(c => c.CacheKeyName.Length > 0).OrderBy(c => c.CacheKeyName).Select(c => c.CacheKeyName).ToList();

            foreach (string itemCacheKeyName in listCacheKeyNames)
            {
                cacheService.RemoveCache(itemCacheKeyName.ToString());
            }

            cacheKeyService.TruncateTableCacheKeysDapper();
        }
    }
}