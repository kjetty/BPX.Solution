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
using System.Linq;
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
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // Get: /Identity/Permit/List
        [Permit(Permits.Identity.Permit.List)]
        public ActionResult List(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            // check input and set defaults
            pageNumber = (pageNumber <= 0) ? 1 : pageNumber;
            pageSize = (pageSize <= 0) ? bpxPageSize : pageSize;
            statusFlag = RecordStatus.Active;   //force set to Active records always
            sortByColumn = (sortByColumn == null || sortByColumn.Trim().Length == 0) ? string.Empty : sortByColumn;
            sortOrder = (sortOrder == null || sortOrder.Trim().Length == 0) ? SortOrder.Ascending : sortOrder;
            searchForString = (searchForString == null || searchForString.Trim().Length == 0) ? string.Empty : searchForString;

            // fetch data
            var model = permitService.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString).Select(c => (PermitMiniViewModel)c);

            // set pagination data
            ViewBag.pageNumber = pageNumber;
            ViewBag.pageSize = pageSize;
            ViewBag.statusFlag = statusFlag;
            ViewBag.sortByColumn = sortByColumn;
            ViewBag.sortOrder = sortOrder;
            ViewBag.searchForString = searchForString;

            return View(model);
        }

        // GET: /Identity/Permit/Create
        [Permit(Permits.Identity.Permit.Create)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Identity/Permit/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Permit.Create)]
        public ActionResult Create(PermitMiniViewModel collection)
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
                    StatusFlag = RecordStatus.Active,
                    ModifiedBy = 1,
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
        public ActionResult Read(int id)
        {
            var model = (PermitViewModel)permitService.GetRecordById(id);
            var modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // GET: /Identity/Permit/Update/5
        [Permit(Permits.Identity.Permit.Update)]
        public ActionResult Update(int id)
        {
            var model = (PermitMiniViewModel)permitService.GetRecordById(id);

            return View(model);
        }

        // POST: /Identity/Permit/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Permit.Update)]
        public ActionResult Update(int id, PermitMiniViewModel collection)
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
                var recordPermit = permitService.GetRecordById(id);

                if (recordPermit.StatusFlag == RecordStatus.Active)
                {
                    // set core data
                    recordPermit.PermitArea = collection.PermitArea;
                    recordPermit.PermitController = collection.PermitController;
                    recordPermit.PermitName = collection.PermitName;
                    recordPermit.PermitEnum = collection.PermitEnum;
                    // set generic data
                    recordPermit.StatusFlag = RecordStatus.Active;
                    recordPermit.ModifiedBy = currUserMeta.UserId;
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
                logger.Log(LogLevel.Error, ex, "PermitController.206");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Update), new { id });
            }
        }

        // GET: /Identity/Permit/Delete/5
        [Permit(Permits.Identity.Permit.Delete)]
        public ActionResult Delete(int id)
        {
            var model = (PermitViewModel)permitService.GetRecordById(id);
            var modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Permit/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Permit.Delete)]
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
                var recordPermit = permitService.GetRecordById(id);

                if (recordPermit.StatusFlag == RecordStatus.Active)
                {
                    // set generic data
                    recordPermit.StatusFlag = RecordStatus.Inactive;
                    recordPermit.ModifiedBy = currUserMeta.UserId;
                    recordPermit.ModifiedDate = DateTime.Now;

                    // edit record
                    permitService.UpdateRecord(recordPermit);
                }

                // commit changes to database
                permitService.SaveDBChanges();

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
                logger.Log(LogLevel.Error, ex, "PermitController.276");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET + POST: /Identity/Permit/ListDeleted
        [Permit(Permits.Identity.Permit.ListDeleted)]
        public ActionResult ListDeleted(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            // check input and set defaults
            pageNumber = (pageNumber <= 0) ? 1 : pageNumber;
            pageSize = (pageSize <= 0) ? bpxPageSize : pageSize;
            statusFlag = RecordStatus.Inactive;   //force set to Active records always
            sortByColumn = (sortByColumn == null || sortByColumn.Trim().Length == 0) ? string.Empty : sortByColumn;
            sortOrder = (sortOrder == null || sortOrder.Trim().Length == 0) ? SortOrder.Ascending : sortOrder;
            searchForString = (searchForString == null || searchForString.Trim().Length == 0) ? string.Empty : searchForString;

            // fetch data
            var model = permitService.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString).Select(c => (PermitMiniViewModel)c);

            // set pagination data
            ViewBag.pageNumber = pageNumber;
            ViewBag.pageSize = pageSize;
            ViewBag.statusFlag = statusFlag;
            ViewBag.sortByColumn = sortByColumn;
            ViewBag.sortOrder = sortOrder;
            ViewBag.searchForString = searchForString;

            return View(model);
        }

        // GET: /Identity/Permit/Undelete/5
        [Permit(Permits.Identity.Permit.Undelete)]
        public ActionResult Undelete(int id)
        {
            var model = (PermitViewModel)permitService.GetRecordById(id);
            var modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Permit/Undelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Permit.Undelete)]
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
                var recordPermit = permitService.GetRecordById(id);

                if (recordPermit.StatusFlag == RecordStatus.Inactive)
                {
                    // set generic data
                    recordPermit.StatusFlag = RecordStatus.Active;
                    recordPermit.ModifiedBy = currUserMeta.UserId;
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
                logger.Log(LogLevel.Error, ex, "PermitController.380");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET: /Identity/Permit/RolesAndMenus/5
        [Permit(Permits.Identity.Permit.Read)]
        public ActionResult RolesAndMenus(int id)
        {
            if (id <= 0)
            {
                // set alert
                ShowAlertBox(AlertType.Error, "Permit Id is not valid.");

                return RedirectToAction(nameof(Index));
            }

            var permit = permitService.GetRecordById(id);
            var listRoleIds = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.PermitId.Equals(id)).Select(c => c.RoleId).ToList();
            var listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && listRoleIds.Contains(c.RoleId)).OrderBy(c => c.RoleName).ToList();
            var listMenuIds = menuPermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.PermitId.Equals(id)).Select(c => c.MenuId).ToList();
            var listMenus = menuService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && listMenuIds.Contains(c.MenuId)).OrderBy(c => c.MenuURL).ToList();

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
            var listCacheKeyNames = cacheKeyService.GetRecordsByFilter(c => c.ModifiedDate >= DateTime.Now.AddMinutes(-240)).OrderBy(c => c.CacheKeyName).Select(c => c.CacheKeyName).ToList();

            foreach (var itemCacheKeyName in listCacheKeyNames)
            {
                cacheService.RemoveCache(itemCacheKeyName.ToString());
            }
        }
    }
}