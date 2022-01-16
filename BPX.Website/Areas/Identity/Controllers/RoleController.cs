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
using X.PagedList;

namespace BPX.Website.Areas.Identity.Controllers
{
	[Area("Identity")]
    public class RoleController : BaseController<RoleController>
    {
        private readonly IRoleService roleService;
        private readonly IPermitService permitService;

        public RoleController(IRoleService roleService, IPermitService permitService)
        {
            this.roleService = roleService;
            this.permitService = permitService;
        }

        // GET: /Identity/Role
        [Permit(Permits.Identity.Role.List)]
        public ActionResult Index()
        {
            // invoke POST
            return Index(1, bpxPageSize, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        // POST: /Identity/Role
        [HttpPost]
        [Permit(Permits.Identity.Role.List)]
        public ActionResult Index(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            // check input and set defaults
            pageNumber = (pageNumber <= 0) ? 1 : pageNumber;
            pageSize = (pageSize <= 0) ? bpxPageSize : pageSize;
            statusFlag = RecordStatus.Active;   //force set to Active records always
            sortByColumn = (sortByColumn == null || sortByColumn.Trim().Length == 0) ? string.Empty : sortByColumn;
            sortOrder = (sortOrder == null || sortOrder.Trim().Length == 0) ? SortOrder.Ascending : sortOrder;
            searchForString = (searchForString == null || searchForString.Trim().Length == 0) ? string.Empty : searchForString;

            // fetch data
            var model = roleService.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString).Select(c => (RoleMiniViewModel)c);

            // set pagination data
            ViewBag.pageNumber = pageNumber;
            ViewBag.pageSize = pageSize;
            ViewBag.statusFlag = statusFlag;
            ViewBag.sortByColumn = sortByColumn;
            ViewBag.sortOrder = sortOrder;
            ViewBag.searchForString = searchForString;

            return View(model);
        }

        // GET: /Identity/Role/Details/5
        [Permit(Permits.Identity.Role.Read)]
        public ActionResult Details(int id)
        {
            var model = (RoleViewModel)roleService.GetRecordByID(id);
            var modelModifiedBy = userService.GetRecordByID(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // GET: /Identity/Role/Create
        [Permit(Permits.Identity.Role.Create)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Identity/Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Role.Create)]
        public ActionResult Create(RoleMiniViewModel collection)
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

                Role recordRole = new()
                {
                    // set core data
                    RoleName = collection.RoleName,
                    RoleDescription = collection.RoleDescription,
                    // set generic data
                    StatusFlag = RecordStatus.Active,
                    ModifiedBy = 1,
                    ModifiedDate = DateTime.Now
                };

                // add record
                roleService.InsertRecord(recordRole);

                // commit changes to database
                roleService.SaveDBChanges();

                // set alert
                ShowAlert(AlertType.Success, "Role is successfully created.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data
                string currControllerAction = "[" + currController + "." + currAction + "]";
                string errorStackTrace = ex.StackTrace.ToString();
                string errorMessage = GetGarneredErrorMessage(ex);

                // log
                logger.Log(LogLevel.Error, currControllerAction + " " + errorMessage + " " + errorStackTrace);

                // set alert
                ShowAlert(AlertType.Error, errorMessage);
                return RedirectToAction(nameof(Create));
            }
        }

        // GET: /Identity/Role/Edit/5
        [Permit(Permits.Identity.Role.Update)]
        public ActionResult Edit(int id)
        {
            var model = (RoleMiniViewModel)roleService.GetRecordByID(id);

            return View(model);
        }

        // POST: /Identity/Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Role.Update)]
        public ActionResult Edit(int id, RoleMiniViewModel collection)
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

                // get existing data
                var recordRole = roleService.GetRecordByID(id);

                if (recordRole.StatusFlag == RecordStatus.Active)
                {
                    // set core data
                    recordRole.RoleName = collection.RoleName;
                    recordRole.RoleDescription = collection.RoleDescription;
                    // set generic data
                    recordRole.StatusFlag = RecordStatus.Active;
                    recordRole.ModifiedBy = 1;
                    recordRole.ModifiedDate = DateTime.Now;

                    // edit record
                    roleService.UpdateRecord(recordRole);
                }

                // commit changes to database
                roleService.SaveDBChanges();

                // set alert
                ShowAlert(AlertType.Success, "Role is successfully updated.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data
                string currControllerAction = "[" + currController + "." + currAction + "]";
                string errorStackTrace = ex.StackTrace.ToString();
                string errorMessage = GetGarneredErrorMessage(ex);

                // log
                logger.Log(LogLevel.Error, currControllerAction + " " + errorMessage + " " + errorStackTrace);

                // set alert
                ShowAlert(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Edit), new { id });
            }
        }

        // GET: /Identity/Role/Delete/5
        [Permit(Permits.Identity.Role.Delete)]
        public ActionResult Delete(int id)
        {
            var model = (RoleViewModel)roleService.GetRecordByID(id);
            var modelModifiedBy = userService.GetRecordByID(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Role/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Role.Delete)]
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
                    ShowAlert(AlertType.Error, modelErrorMessage);

                    return View(collection);
                }

                // get existing data
                var recordRole = roleService.GetRecordByID(id);

                if (recordRole.StatusFlag == RecordStatus.Active)
                {
                    // set generic data
                    recordRole.StatusFlag = RecordStatus.Inactive;
                    recordRole.ModifiedBy = 1;
                    recordRole.ModifiedDate = DateTime.Now;

                    // edit record
                    roleService.UpdateRecord(recordRole);
                }

                // commit changes to database
                roleService.SaveDBChanges();

                // set alert
                ShowAlert(AlertType.Success, "Role is successfully deleted.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data
                string currControllerAction = "[" + currController + "." + currAction + "]";
                string errorStackTrace = ex.StackTrace.ToString();
                string errorMessage = GetGarneredErrorMessage(ex);

                // log
                logger.Log(LogLevel.Error, currControllerAction + " " + errorMessage + " " + errorStackTrace);

                // set alert
                ShowAlert(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET: /Identity/Role/ListDeleted
        [Permit(Permits.Identity.Role.Delete)]
        public ActionResult ListDeleted()
        {
            return ListDeleted(1, bpxPageSize, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        // POST: /Identity/Role/ListDeleted
        [HttpPost]
        //[Permit(Permits.Identity.Role.Delete)]
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
            var model = roleService.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString).Select(c => (RoleMiniViewModel)c);

            // set pagination data
            ViewBag.pageNumber = pageNumber;
            ViewBag.pageSize = pageSize;
            ViewBag.statusFlag = statusFlag;
            ViewBag.sortByColumn = sortByColumn;
            ViewBag.sortOrder = sortOrder;
            ViewBag.searchForString = searchForString;

            return View(model);
        }

        // GET: /Identity/Role/Undelete/5
        [Permit(Permits.Identity.Role.Restore)]
        public ActionResult Undelete(int id)
        {
            var model = (RoleViewModel)roleService.GetRecordByID(id);
            var modelModifiedBy = userService.GetRecordByID(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Role/Undelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Role.Restore)]
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
                    ShowAlert(AlertType.Error, modelErrorMessage);

                    return View(collection);
                }

                // get existing data
                var recordRole = roleService.GetRecordByID(id);

                if (recordRole.StatusFlag == RecordStatus.Inactive)
                {
                    // set generic data
                    recordRole.StatusFlag = RecordStatus.Active;
                    recordRole.ModifiedBy = 1;
                    recordRole.ModifiedDate = DateTime.Now;

                    // edit record
                    roleService.UpdateRecord(recordRole);
                }

                // commit changes to database
                roleService.SaveDBChanges();

                // set alert
                ShowAlert(AlertType.Success, "Role is successfully restored.");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // prepare data
                string currControllerAction = "[" + currController + "." + currAction + "]";
                string errorStackTrace = ex.StackTrace.ToString();
                string errorMessage = GetGarneredErrorMessage(ex);

                // log
                logger.Log(LogLevel.Error, currControllerAction + " " + errorMessage + " " + errorStackTrace);

                // set alert
                ShowAlert(AlertType.Error, errorMessage);

                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // GET: /Identity/Role/Permit/5
        [Permit(Permits.Identity.RolePermit.CRUD)]
        public ActionResult Permit(int id)
        {
            var role = roleService.GetRecordByID(id);
            string cacheKey = string.Empty;

            //listPermits
            cacheKey = "permits:all";
            List<Permit> listPermits = listPermits = bpxCache.GetCache<List<Permit>>(cacheKey);

            if (listPermits == null)
            {
                listPermits = permitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active)).OrderBy(c => c.PermitArea).ThenBy(c => c.PermitController).ThenBy(c => c.PermitName).ToList();
                bpxCache.SetCache(listPermits, cacheKey, memoryCacheKeyService);
            }

            //listRolePermitIDs
            cacheKey = $"role:{id}:permits";
            List<int> listRolePermitIDs = bpxCache.GetCache<List<int>>(cacheKey);

            if (listRolePermitIDs == null)
            {
                listRolePermitIDs = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.RoleId.Equals(id)).OrderBy(c => c.PermitID).Select(c => c.PermitID).ToList();
                bpxCache.SetCache(listRolePermitIDs, cacheKey, memoryCacheKeyService);
            }

            //listPermitAreas
            cacheKey = $"permitareas:all";
            List<string> listAreas = bpxCache.GetCache<List<string>>(cacheKey);

            if (listAreas == null)
            {
                listAreas = listPermits.OrderBy(c => c.PermitArea).Select(c => c.PermitArea).Distinct().ToList();
                bpxCache.SetCache(listAreas, cacheKey, memoryCacheKeyService);
            }

            listPermits = permitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active)).OrderBy(c => c.PermitArea).ThenBy(c => c.PermitController).ThenBy(c => c.PermitName).ToList();
            listRolePermitIDs = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.RoleId.Equals(id)).OrderBy(c => c.PermitID).Select(c => c.PermitID).ToList();
            listAreas = listPermits.OrderBy(c => c.PermitArea).Select(c => c.PermitArea).Distinct().ToList();

            // set ViewBag
            ViewBag.role = role;
            ViewBag.listAreas = listAreas;
            ViewBag.listPermits = listPermits;
            ViewBag.listRolePermitIDs = listRolePermitIDs;

            return View();
        }

        // GET: /Identity/Role/Permit/5
        [HttpPost]
        [Permit(Permits.Identity.RolePermit.CRUD)]
        public ActionResult Permit(int id, List<int> permitIDs)
        {
            var listRolePermits = rolePermitService.GetRecordsByFilter(c => c.RoleId == id).ToList();

            foreach (var rolePermit in listRolePermits)
            {
                rolePermit.StatusFlag = RecordStatus.Inactive;
                rolePermit.ModifiedBy = 1;
                rolePermit.ModifiedDate = DateTime.Now;
            }

            rolePermitService.SaveDBChanges();

            foreach (var permitID in permitIDs)
            {
                var existingRolePermit = rolePermitService.GetRecordsByFilter(c => c.RoleId == id && c.PermitID == permitID).FirstOrDefault();

                if (existingRolePermit != null)
                {
                    existingRolePermit.StatusFlag = RecordStatus.Active;
                    existingRolePermit.ModifiedBy = 1;
                    existingRolePermit.ModifiedDate = DateTime.Now;

                    rolePermitService.UpdateRecord(existingRolePermit);
                }
                else
                {
                    RolePermit newRolePermit = new()
                    {
                        RoleId = id,
                        PermitID = permitID,
                        StatusFlag = RecordStatus.Active,
                        ModifiedBy = 1,
                        ModifiedDate = DateTime.Now
                    };

                    rolePermitService.InsertRecord(newRolePermit);
                }
            }

            rolePermitService.SaveDBChanges();

            //// remove from cache
            //// get ROLES associated with the USER (from DB)
            //var userRolesList = userRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.UserId == currUserId).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
            //string cacheKey = $"urp{string.Join(string.Empty, userRolesList)}";

            ////memoryCache.Remove(cacheKey);

            // set alert
            ShowAlert(AlertType.Success, "Role Permits are successfully updated.");

            //return Permit(id);
            return RedirectToAction(nameof(Index));
        }
    }
}