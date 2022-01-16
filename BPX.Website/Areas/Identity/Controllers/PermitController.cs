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
        private readonly IPermitService permitService;
        private readonly IRoleService roleService;


        public PermitController(IPermitService permitService, IRoleService roleService)
        {
            this.permitService = permitService;
            this.roleService = roleService;
        }

        // GET: /Identity/Permit
        [Permit(Permits.Identity.Permit.List)]
        public ActionResult Index()
        {
            // invoke POST
            return Index(1, bpxPageSize, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        // POST: /Identity/Permit
        [HttpPost]
        [Permit(Permits.Identity.Permit.List)]
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

        // GET: /Identity/Permit/Details/5
        [Permit(Permits.Identity.Permit.Read)]
        public ActionResult Details(int id)
        {
            var model = (PermitViewModel)permitService.GetRecordByID(id);
            var modelModifiedBy = userService.GetRecordByID(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

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
                    ShowAlert(AlertType.Error, modelErrorMessage);

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
                ShowAlert(AlertType.Success, "Permit is successfully created.");

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

        // GET: /Identity/Permit/Edit/5
        [Permit(Permits.Identity.Permit.Update)]
        public ActionResult Edit(int id)
        {
            var model = (PermitMiniViewModel)permitService.GetRecordByID(id);

            return View(model);
        }

        // POST: /Identity/Permit/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Permit.Update)]
        public ActionResult Edit(int id, PermitMiniViewModel collection)
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
                var recordPermit = permitService.GetRecordByID(id);

                if (recordPermit.StatusFlag == RecordStatus.Active)
                {
                    // set core data
                    recordPermit.PermitArea = collection.PermitArea;
                    recordPermit.PermitController = collection.PermitController;
                    recordPermit.PermitName = collection.PermitName;
                    recordPermit.PermitEnum = collection.PermitEnum;
                    // set generic data
                    recordPermit.StatusFlag = RecordStatus.Active;
                    recordPermit.ModifiedBy = 1;
                    recordPermit.ModifiedDate = DateTime.Now;

                    // edit record
                    permitService.UpdateRecord(recordPermit);
                }

                // commit changes to database
                permitService.SaveDBChanges();

                // set alert
                ShowAlert(AlertType.Success, "Permit is successfully updated.");

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

        // GET: /Identity/Permit/Delete/5
        [Permit(Permits.Identity.Permit.Delete)]
        public ActionResult Delete(int id)
        {
            var model = (PermitViewModel)permitService.GetRecordByID(id);
            var modelModifiedBy = userService.GetRecordByID(model.ModifiedBy);

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
                    ShowAlert(AlertType.Error, modelErrorMessage);

                    return View(collection);
                }

                // get existing data
                var recordPermit = permitService.GetRecordByID(id);

                if (recordPermit.StatusFlag == RecordStatus.Active)
                {
                    // set generic data
                    recordPermit.StatusFlag = RecordStatus.Inactive;
                    recordPermit.ModifiedBy = 1;
                    recordPermit.ModifiedDate = DateTime.Now;

                    // edit record
                    permitService.UpdateRecord(recordPermit);
                }

                // commit changes to database
                permitService.SaveDBChanges();

                // set alert
                ShowAlert(AlertType.Success, "Permit is successfully deleted.");

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

        // GET: /Identity/Permit/ListDeleted
        [Permit(Permits.Identity.Permit.ListDeleted)]
        public ActionResult ListDeleted()
        {
            return ListDeleted(1, bpxPageSize, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        // POST: /Identity/Permit/ListDeleted
        [HttpPost]
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
        [Permit(Permits.Identity.Permit.Restore)]
        public ActionResult Undelete(int id)
        {
            var model = (PermitViewModel)permitService.GetRecordByID(id);
            var modelModifiedBy = userService.GetRecordByID(model.ModifiedBy);

            // set ViewBag
            ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

            return View(model);
        }

        // POST: /Identity/Permit/Undelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.Permit.Restore)]
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
                var recordPermit = permitService.GetRecordByID(id);

                if (recordPermit.StatusFlag == RecordStatus.Inactive)
                {
                    // set generic data
                    recordPermit.StatusFlag = RecordStatus.Active;
                    recordPermit.ModifiedBy = 1;
                    recordPermit.ModifiedDate = DateTime.Now;

                    // edit record
                    permitService.UpdateRecord(recordPermit);
                }

                // commit changes to database
                permitService.SaveDBChanges();

                // set alert
                ShowAlert(AlertType.Success, "Permit is successfully restored.");

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
    }
}