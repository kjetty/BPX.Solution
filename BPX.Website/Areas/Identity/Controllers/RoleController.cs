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
    public class RoleController : BaseController<RoleController>
    {
        private readonly ICacheService cacheService;
        private readonly ICacheKeyService cacheKeyService;
        private readonly IUserService userService;
        private readonly IRoleService roleService;
        private readonly IPermitService permitService;
        private readonly IRolePermitService rolePermitService;

        public RoleController(ILogger<RoleController> logger, ICoreService coreService, IRoleService roleService) : base(logger, coreService)
        {
            this.cacheService = coreService.GetCacheService();
            this.cacheKeyService = coreService.GetCacheKeyService();
            this.userService = coreService.GetUserService();
            this.roleService = roleService; 
            this.permitService = coreService.GetPermitService();
            this.rolePermitService = coreService.GetRolePermitService();
        }

        // GET: /Identity/Role
        [Permit(Permits.Identity.Role.List)]
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: /Identity/Role/List
        [Permit(Permits.Identity.Role.List)]
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
            IPagedList<RoleMiniViewModel> model = roleService.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString, filterJson).Select(c => (RoleMiniViewModel)c);

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

        // GET: /Identity/Role/Create
        [Permit(Permits.Identity.Role.Create)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Identity/Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Role.Create)]
        public IActionResult Create(RoleMiniViewModel collection)
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

                Role recordRole = new()
                {
                    // set core data
                    RoleName = collection.RoleName,
                    RoleDescription = collection.RoleDescription,
                    // set generic data
                    StatusFlag = RecordStatus.Active.ToUpper(),
                    ModifiedBy = currUser.UserId,
                    ModifiedDate = DateTime.Now
                };

                // add record
                roleService.InsertRecord(recordRole);

                // commit changes to database
                roleService.SaveDBChanges();

                // set alert
                ShowAlertBox(AlertType.Success, "Role is successfully created.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "RoleController.125");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);
                return RedirectToAction(nameof(Create));
            }
        }

        // GET: /Identity/Role/Read/5
        [Permit(Permits.Identity.Role.Read)]
        public IActionResult Read(int id)
        {
            RoleViewModel model = (RoleViewModel)roleService.GetRecordById(id);
            User modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // GET: /Identity/Role/Update/5
        [Permit(Permits.Identity.Role.Update)]
        public IActionResult Update(int id)
        {
            RoleMiniViewModel model = (RoleMiniViewModel)roleService.GetRecordById(id);

            return View(model);
        }

        // POST: /Identity/Role/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Role.Update)]
        public IActionResult Update(int id, RoleMiniViewModel collection)
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

                using (var scope = new TransactionScope())
                {
                    // get existing data
                    Role recordRole = roleService.GetRecordById(id);

                    if (recordRole.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()))
                    {
                        // set core data
                        recordRole.RoleName = collection.RoleName.Trim();
                        recordRole.RoleDescription = collection.RoleDescription.Trim();
                        // set generic data
                        recordRole.StatusFlag = RecordStatus.Active.ToUpper();
                        recordRole.ModifiedBy = currUser.UserId;
                        recordRole.ModifiedDate = DateTime.Now;

                        // edit record
                        roleService.UpdateRecord(recordRole);
                    }

                    // commit changes to database
                    roleService.SaveDBChanges();
                }

                // reset cache
                ResetCache();

                // set alert
                ShowAlertBox(AlertType.Success, "Role is successfully updated.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "RoleController.209");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Update), new { id });
            }
        }

        // GET: /Identity/Role/Delete/5
        [Permit(Permits.Identity.Role.Delete)]
        public IActionResult Delete(int id)
        {
            RoleViewModel model = (RoleViewModel)roleService.GetRecordById(id);
            User modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Role/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Role.Delete)]
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
                Role recordRole = roleService.GetRecordById(id);

                if (recordRole.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()))
                {
                    // set generic data
                    recordRole.StatusFlag = RecordStatus.Inactive.ToUpper();
                    recordRole.ModifiedBy = currUser.UserId;
                    recordRole.ModifiedDate = DateTime.Now;

                    // edit record
                    roleService.UpdateRecord(recordRole);
                }

                // commit changes to database
                roleService.SaveDBChanges();

                // reset cache
                ResetCache();

                // set alert
                ShowAlertBox(AlertType.Success, "Role is successfully deleted.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "RoleController.282");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET + POST: /Identity/Role/ListDeleted
        [Permit(Permits.Identity.Role.ListDeleted)]
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
            IPagedList<RoleMiniViewModel> model = roleService.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString, filterJson).Select(c => (RoleMiniViewModel)c);

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

        // GET: /Identity/Role/Undelete/5
        [Permit(Permits.Identity.Role.Undelete)]
        public IActionResult Undelete(int id)
        {
            RoleViewModel model = (RoleViewModel)roleService.GetRecordById(id);
            User modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Role/Undelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Role.Undelete)]
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
                Role recordRole = roleService.GetRecordById(id);

                if (recordRole.StatusFlag.Equals(RecordStatus.Inactive.ToUpper()))
                {
                    // set generic data
                    recordRole.StatusFlag = RecordStatus.Active.ToUpper();
                    recordRole.ModifiedBy = currUser.UserId;
                    recordRole.ModifiedDate = DateTime.Now;

                    // edit record
                    roleService.UpdateRecord(recordRole);
                }

                // commit changes to database
                roleService.SaveDBChanges();

                // reset cache
                ResetCache();

                // set alert
                ShowAlertBox(AlertType.Success, "Role is successfully restored.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data				
                string errorMessage = GetInnerExceptionMessage(ex);

                // log
                logger.Log(LogLevel.Error, ex, "RoleController.382");

                // set alert
                ShowAlertBox(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET: /Identity/Role/Permit/5
        [Permit(Permits.Identity.Role.RolePermits)]
        public IActionResult RolePermits(int id)
        {
            if (id <= 0)
            {
                // set alert
                ShowAlertBox(AlertType.Error, "Role Id is not valid.");

                return RedirectToAction(nameof(Index));
            }

            Role role = roleService.GetRecordById(id);
            List<Permit> listPermits = permitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).OrderBy(c => c.PermitArea).ThenBy(c => c.PermitController).ThenBy(c => c.PermitName).ToList();
            List<int> listRolePermitIds = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.RoleId.Equals(id)).OrderBy(c => c.PermitId).Select(c => c.PermitId).ToList();
            List<string> listAreas = listPermits.OrderBy(c => c.PermitArea).Select(c => c.PermitArea).Distinct().ToList();

            // set ViewBag
            ViewBag.role = role;
            ViewBag.listAreas = listAreas;
            ViewBag.listPermits = listPermits;
            ViewBag.listRolePermitIds = listRolePermitIds;

            return View();
        }

        // GET: /Identity/Role/Permit/5
        [HttpPost]
        [Permit(Permits.Identity.Role.RolePermits)]
        public IActionResult RolePermits(int id, List<int> permitIds)
        {
            // get all existing active permits for the role
            List<RolePermit> listRolePermits = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.RoleId.Equals(id)).ToList();

            // delete all existing active permits for the role
            foreach (RolePermit rolePermit in listRolePermits)
            {
                rolePermit.StatusFlag = RecordStatus.Inactive.ToUpper();
                rolePermit.ModifiedBy = currUser.UserId;
                rolePermit.ModifiedDate = DateTime.Now;

                rolePermitService.UpdateRecord(rolePermit);
            }

            rolePermitService.SaveDBChanges();

            // add or activate received permits for the role
            foreach (int permitId in permitIds)
            {
                RolePermit existingRolePermit = rolePermitService.GetRecordsByFilter(c => c.RoleId.Equals(id) && c.PermitId.Equals(permitId)).SingleOrDefault();

                if (existingRolePermit != null)
                {
                    existingRolePermit.StatusFlag = RecordStatus.Active.ToUpper();
                    existingRolePermit.ModifiedBy = currUser.UserId;
                    existingRolePermit.ModifiedDate = DateTime.Now;

                    rolePermitService.UpdateRecord(existingRolePermit);
                }
                else
                {
                    RolePermit newRolePermit = new()
                    {
                        RoleId = id,
                        PermitId = permitId,
                        StatusFlag = RecordStatus.Active.ToUpper(),
                        ModifiedBy = currUser.UserId,
                        ModifiedDate = DateTime.Now
                    };

                    rolePermitService.InsertRecord(newRolePermit);
                }
            }

            rolePermitService.SaveDBChanges();

            // reset cache
            ResetCache();

            // set alert
            ShowAlertBox(AlertType.Success, "Role Permits are successfully updated.");

            //return Permit(id);
            return RedirectToAction(nameof(Index));
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