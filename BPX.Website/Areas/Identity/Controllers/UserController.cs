using BPX.Domain.DbModels;
using BPX.Domain.ViewModels;
using BPX.Service;
using BPX.Utils;
using BPX.Website.Controllers;
using BPX.Website.CustomCode.Authorize;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using X.PagedList;

namespace BPX.Website.Areas.Identity.Controllers
{
	[Area("Identity")]
    public class UserController : BaseController<UserController>
    {
		private readonly ICacheService cacheService;
		private readonly ICacheKeyService cacheKeyService;
		private readonly IUserService userService;
		private readonly IUserRoleService userRoleService;
		private readonly IRoleService roleService;

        public UserController(ILogger<UserController> logger, ICoreService coreService, IRoleService roleService) : base(logger, coreService)
        {
			this.cacheService = coreService.GetCacheService();
			this.cacheKeyService = coreService.GetCacheKeyService();
			this.userService = coreService.GetUserService();
			this.userRoleService = coreService.GetUserRoleService();
			this.roleService = roleService;
		}

		// GET: /Identity/User
		[Permit(Permits.Identity.User.List)]
		public ActionResult Index()
		{
			return RedirectToAction("List");
		}

        // GET: /Identity/User/List
        [Permit(Permits.Identity.User.List)]
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
			var model = userService.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString).Select(c => (UserMiniViewModel)c);
			var listUserId = model.Select(c => c.UserId).ToList();
			var listUsersRoles = userRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && listUserId.Contains(c.UserId));
			var listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active));

			// set ViewBag
			ViewBag.listUsersRoles = listUsersRoles;
			ViewBag.listRoles = listRoles;

			// set pagination data
			ViewBag.pageNumber = pageNumber;
			ViewBag.pageSize = pageSize;
			ViewBag.statusFlag = statusFlag;
			ViewBag.sortByColumn = sortByColumn;
			ViewBag.sortOrder = sortOrder;
			ViewBag.searchForString = searchForString;

			return View(model);
		}

        // GET: /Identity/User/Read/5
        [Permit(Permits.Identity.User.Read)]
        public ActionResult Read(int id)
        {
			//todo
			//restrict details view to current user and user with .List permit

			var model = (UserViewModel)userService.GetRecordById(id);
			var listUsersRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));
			var listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active));
			var modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

			// set ViewBag
			ViewBag.listUsersRoles = listUsersRoles;
			ViewBag.listRoles = listRoles;
			ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

			return View(model);
		}

        // GET: /Identity/User/Update/5
        [Permit(Permits.Identity.User.Update)]
        public ActionResult Update(int id)
        {
			var model = (UserMiniViewModel)userService.GetRecordById(id);

			var listUsersRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));
			var listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active));

			ViewBag.listUsersRoles = listUsersRoles;
			ViewBag.listRoles = listRoles;

			return View(model);
		}

        // POST: /Identity/User/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.User.Update)]
        public ActionResult Update(int id, UserMiniViewModel collection)
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
				var recordUser = userService.GetRecordById(id);
				//var listUserRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));

				if (recordUser.StatusFlag == RecordStatus.Active)
				{
					// set core data
					recordUser.FirstName = collection.FirstName;
					recordUser.LastName = collection.LastName;
					recordUser.Email = collection.Email;
					recordUser.Mobile = collection.Mobile;
					// set generic data
					recordUser.StatusFlag = RecordStatus.Active;
					recordUser.ModifiedBy = currUserMeta.UserId;
					recordUser.ModifiedDate = DateTime.Now;

					// edit record
					userService.UpdateRecord(recordUser);
				}

				// commit changes to database
				userService.SaveDBChanges();

				// reset cache
				ResetCache(id);

				// set alert
				ShowAlertBox(AlertType.Success, "User is successfully updated.");

				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				// prepare data				
				string errorMessage = GetInnerExceptionMessage(ex);

				// log
				logger.Log(LogLevel.Error, ex, "UserController.164");

				// set alert
				ShowAlertBox(AlertType.Error, errorMessage);

				return RedirectToAction(nameof(Update), new { id });
			}
		}

        // GET: /Identity/User/Delete/5
        [Permit(Permits.Identity.User.Delete)]
        public ActionResult Delete(int id)
        {
			var model = (UserViewModel)userService.GetRecordById(id);
			var listUsersRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));
			var listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active));
			var modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

			// set ViewBag
			ViewBag.listUsersRoles = listUsersRoles;
			ViewBag.listRoles = listRoles;
			ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

			return View(model);
		}

		// POST: /Identity/User/Delete/5
		[HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.User.Delete)]
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
				var recordUser = userService.GetRecordById(id);
				var listUserRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));

				if (recordUser.StatusFlag == RecordStatus.Active)
				{
					// set generic data
					recordUser.StatusFlag = RecordStatus.Inactive;
					recordUser.ModifiedBy = currUserMeta.UserId;
					recordUser.ModifiedDate = DateTime.Now;

					// edit record
					userService.UpdateRecord(recordUser);
				}

				// commit changes to database
				userService.SaveDBChanges();

				// reset cache
				ResetCache(id);

				// set alert
				ShowAlertBox(AlertType.Success, "User is successfully deleted.");

				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				// prepare data				
				string errorMessage = GetInnerExceptionMessage(ex);

				// log
				logger.Log(LogLevel.Error, ex, "UserController.247");

				// set alert
				ShowAlertBox(AlertType.Error, errorMessage);

				return RedirectToAction(nameof(Delete), new { id });
			}
		}
		
        // GET + POST: /Identity/User/ListDeleted
        [Permit(Permits.Identity.User.ListDeleted)]
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
			var model = userService.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString).Select(c => (UserMiniViewModel)c);
			var listUserId = model.Select(c => c.UserId).ToList();
			var listUsersRoles = userRoleService.GetRecordsByFilter(c => listUserId.Contains(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));
			var listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active));

			// set ViewBag
			ViewBag.listUsersRoles = listUsersRoles;
			ViewBag.listRoles = listRoles;

			// set pagination data
			ViewBag.pageNumber = pageNumber;
			ViewBag.pageSize = pageSize;
			ViewBag.statusFlag = statusFlag;
			ViewBag.sortByColumn = sortByColumn;
			ViewBag.sortOrder = sortOrder;
			ViewBag.searchForString = searchForString;

			return View(model);
		}

        // GET: /Identity/User/Undelete/5
        [Permit(Permits.Identity.User.Undelete)]
        public ActionResult Undelete(int id)
        {
			var model = (UserViewModel)userService.GetRecordById(id);
			var listUsersRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));
			var listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active));
			var modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

			// set ViewBag
			ViewBag.listUsersRoles = listUsersRoles;
			ViewBag.listRoles = listRoles;
			ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

			return View(model);
		}

        // POST: /Identity/User/Undelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.User.Undelete)]
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
				var recordUser = userService.GetRecordById(id);
				var listUserRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));

				if (recordUser.StatusFlag == RecordStatus.Inactive)
				{
					// set generic data
					recordUser.StatusFlag = RecordStatus.Active;
					recordUser.ModifiedBy = currUserMeta.UserId;
					recordUser.ModifiedDate = DateTime.Now;

					// edit record
					userService.UpdateRecord(recordUser);
				}

				// commit changes to database
				userService.SaveDBChanges();

				// reset cache
				ResetCache(id);

				// set alert
				ShowAlertBox(AlertType.Success, "User is successfully restored.");

				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				// prepare data				
				string errorMessage = GetInnerExceptionMessage(ex);

				// log
				logger.Log(LogLevel.Error, ex, "UserController.363");

				// set alert
				ShowAlertBox(AlertType.Error, errorMessage);

				return RedirectToAction(nameof(Delete), new { id });
			}
		}

		// GET: /Identity/User/Role/5
		[Permit(Permits.Identity.UserRole.CRUD)]
		public ActionResult Role(int id)
		{
			if (id <= 0)
			{
				// set alert
				ShowAlertBox(AlertType.Error, "User Id is not valid.");

				return RedirectToAction(nameof(Index));
			}

			var user = userService.GetRecordById(id);
			var listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active)).OrderBy(c => c.RoleName).ToList();
			var listUserRoleIds = userRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.UserId.Equals(id)).OrderBy(c => c.RoleId).Select(c => c.RoleId).ToList();
			
			// set ViewBag
			ViewBag.user = user;
			ViewBag.listRoles = listRoles;
			ViewBag.listUserRoleIds = listUserRoleIds;

			return View();
		}

		// GET: /Identity/User/Role/5
		[HttpPost]
		[Permit(Permits.Identity.UserRole.CRUD)]
		public ActionResult Role(int id, List<int> roleIds)
		{
			var listUserRoles = userRoleService.GetRecordsByFilter(c => c.UserId == id).ToList();

			// delete all roles for the user
			foreach (var userRole in listUserRoles)
			{
				userRole.StatusFlag = RecordStatus.Inactive;
				userRole.ModifiedBy = currUserMeta.UserId;
				userRole.ModifiedDate = DateTime.Now;
			}

			userRoleService.SaveDBChanges();

			// add or activate received roles for the user
			foreach (var roleId in roleIds)
			{
				var existingUserRole = userRoleService.GetRecordsByFilter(c => c.UserId == id && c.RoleId == roleId).FirstOrDefault();

				if (existingUserRole != null)
				{
					existingUserRole.StatusFlag = RecordStatus.Active;
					existingUserRole.ModifiedBy = currUserMeta.UserId;
					existingUserRole.ModifiedDate = DateTime.Now;

					userRoleService.UpdateRecord(existingUserRole);
				}
				else
				{
					UserRole newUserRole = new()
					{
						UserId = id,
						RoleId = roleId,
						StatusFlag = RecordStatus.Active,
						ModifiedBy = 1,
						ModifiedDate = DateTime.Now
					};

					userRoleService.InsertRecord(newUserRole);
				}
			}

			userRoleService.SaveDBChanges();

			// reset cache
			ResetCache(id);

			// set alert
			ShowAlertBox(AlertType.Success, "User Roles are successfully updated.");

			return RedirectToAction(nameof(Index));
		}
	
		private void ResetCache(int id)
		{
			//// cache :: remove following :: 
			// $"user:{userId}:meta";
			// $"user:{userId}:roles";
			// $"user:{userId}:permits";

			var listCacheKeyNames = cacheKeyService.GetRecordsByFilter(c => c.CacheKeyName.Contains($"user:{id}:") && c.ModifiedDate >= DateTime.Now.AddMinutes(-240)).OrderBy(c => c.CacheKeyName).Select(c => c.CacheKeyName).ToList();

			foreach (var itemCacheKeyName in listCacheKeyNames)
			{
				cacheService.RemoveCache(itemCacheKeyName.ToString());
			}
		}
	}
}