﻿using BPX.Domain.DbModels;
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
    public class UserController : BaseController<UserController>
    {
        private readonly IRoleService roleService;

        public UserController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        // GET: /Identity/User
        [Permit(Permits.Identity.User.List)]
        public ActionResult Index()
        {
			// invoke POST
			return Index(1, bpxPageSize, string.Empty, string.Empty, string.Empty, string.Empty);
		}

        // POST: /Identity/User
        [HttpPost]
        [Permit(Permits.Identity.User.List)]
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

        // GET: /Identity/User/Details/5
        [Permit(Permits.Identity.User.Read)]
        public ActionResult Details(int id)
        {
			//todo
			//restrict details view to current user and user with .List permit

			var model = (UserViewModel)userService.GetRecordByID(id);
			var listUsersRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));
			var listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active));
			var modelModifiedBy = userService.GetRecordByID(model.ModifiedBy);

			// set ViewBag
			ViewBag.listUsersRoles = listUsersRoles;
			ViewBag.listRoles = listRoles;
			ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

			return View(model);
		}

        // GET: /Identity/User/Edit/5
        [Permit(Permits.Identity.User.Update)]
        public ActionResult Edit(int id)
        {
			var model = (UserMiniViewModel)userService.GetRecordByID(id);

			var listUsersRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));
			var listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active));

			ViewBag.listUsersRoles = listUsersRoles;
			ViewBag.listRoles = listRoles;

			return View(model);
		}

        // POST: /Identity/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.User.Update)]
        public ActionResult Edit(int id, UserMiniViewModel collection)
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
				var recordUser = userService.GetRecordByID(id);
				var listUserRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));

				if (recordUser.StatusFlag == RecordStatus.Active)
				{
					// set core data
					recordUser.FirstName = collection.FirstName;
					recordUser.LastName = collection.LastName;
					recordUser.Email = collection.Email;
					recordUser.Mobile = collection.Mobile;
					// set generic data
					recordUser.StatusFlag = RecordStatus.Active;
					recordUser.ModifiedBy = 1;
					recordUser.ModifiedDate = DateTime.Now;

					// edit record
					userService.UpdateRecord(recordUser);
				}

				// delete (deactivate) all roles currently associated with the user
				foreach (var itemUserRole in listUserRoles)
				{
					// set generic data
					itemUserRole.StatusFlag = RecordStatus.Inactive;
					itemUserRole.ModifiedBy = 1;
					itemUserRole.ModifiedDate = DateTime.Now;

					// edit record
					userRoleService.UpdateRecord(itemUserRole);
				}

				// activate the received roles for the user
				if (collection.UserRoleIds != null)
				{
					foreach (var itemRoleId in collection.UserRoleIds)
					{
						int tempRoleId = Convert.ToInt32(itemRoleId);
						var rowUserRole = userRoleService.GetRecordsByFilter(c => c.UserId == id && c.RoleId == tempRoleId).SingleOrDefault();

						if (rowUserRole != null)
						{
							// update (reactivate) existing userRole data
							// set generic data
							rowUserRole.StatusFlag = RecordStatus.Active;
							rowUserRole.ModifiedBy = 1;
							rowUserRole.ModifiedDate = DateTime.Now;

							// edit record
							userRoleService.UpdateRecord(rowUserRole);
						}
						else
						{
							//add new userRole data
							UserRole userRole = new()
							{
								// set core data
								UserId = id,
								RoleId = tempRoleId,
								// set generic data
								StatusFlag = RecordStatus.Active,
								ModifiedBy = 1,
								ModifiedDate = DateTime.Now
							};

							// add record
							userRoleService.InsertRecord(userRole);
						}
					}
				}

				// commit changes to database
				userService.SaveDBChanges();
				userRoleService.SaveDBChanges();

				// set alert
				ShowAlert(AlertType.Success, "User is successfully updated.");

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

        // GET: /Identity/User/Delete/5
        [Permit(Permits.Identity.User.Delete)]
        public ActionResult Delete(int id)
        {
			var model = (UserViewModel)userService.GetRecordByID(id);
			var listUsersRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));
			var listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active));
			var modelModifiedBy = userService.GetRecordByID(model.ModifiedBy);

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
					ShowAlert(AlertType.Error, modelErrorMessage);

					return View(collection);
				}

				// get existing data
				var recordUser = userService.GetRecordByID(id);
				var listUserRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));

				if (recordUser.StatusFlag == RecordStatus.Active)
				{
					// set generic data
					recordUser.StatusFlag = RecordStatus.Inactive;
					recordUser.ModifiedBy = 1;
					recordUser.ModifiedDate = DateTime.Now;

					// edit record
					userService.UpdateRecord(recordUser);
				}

				// commit changes to database
				userService.SaveDBChanges();

				// handle cache :: remove from cache using keys from the database
				var memoryCacheKeys = memoryCacheKeyService.GetRecordsByFilter(c => c.CacheKey.Contains($"user:{id}:") && c.ModifiedDate >= DateTime.Now.AddMinutes(-30)).OrderBy(c => c.CacheKey).ToList();

				foreach(var memoryCacheKey in memoryCacheKeys)
				{
					bpxCache.RemoveCache(memoryCacheKey.CacheKey);
				}

				// set alert
				ShowAlert(AlertType.Success, "User is successfully deleted.");

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

        // GET: /Identity/User/ListDeleted
        [Permit(Permits.Identity.User.ListDeleted)]
        public ActionResult ListDeleted()
        {
            return ListDeleted(1, bpxPageSize, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        // POST: /Identity/User/ListDeleted
        [HttpPost]
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
        [Permit(Permits.Identity.User.Restore)]
        public ActionResult Undelete(int id)
        {
			var model = (UserViewModel)userService.GetRecordByID(id);
			var listUsersRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));
			var listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active));
			var modelModifiedBy = userService.GetRecordByID(model.ModifiedBy);

			// set ViewBag
			ViewBag.listUsersRoles = listUsersRoles;
			ViewBag.listRoles = listRoles;
			ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

			return View(model);
		}

        // POST: /Identity/User/Undelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.User.Restore)]
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
				var recordUser = userService.GetRecordByID(id);
				var listUserRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.Equals(RecordStatus.Active));

				if (recordUser.StatusFlag == RecordStatus.Inactive)
				{
					// set generic data
					recordUser.StatusFlag = RecordStatus.Active;
					recordUser.ModifiedBy = 1;
					recordUser.ModifiedDate = DateTime.Now;

					// edit record
					userService.UpdateRecord(recordUser);
				}

				// commit changes to database
				userService.SaveDBChanges();

				// set alert
				ShowAlert(AlertType.Success, "User is successfully restored.");

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

		// GET: /Identity/User/Role/5
		[Permit(Permits.Identity.UserRole.CRUD)]
		public ActionResult Role(int id)
		{
			var user = userService.GetRecordByID(id);
			string cacheKey = string.Empty;

			// listRoles
			cacheKey = "roles:all";
			List<Role> listRoles = listRoles = bpxCache.GetCache<List<Role>>(cacheKey);

			if (listRoles == null)
			{
				listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active)).OrderBy(c => c.RoleName).ToList();
				bpxCache.SetCache(listRoles, cacheKey, memoryCacheKeyService);
			}

			// listUserRoleIds
			cacheKey = $"user:{id}:roles";
			List<int> listUserRoleIds = bpxCache.GetCache<List<int>>(cacheKey);

			if (listUserRoleIds == null)
			{
				listUserRoleIds = userRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.UserId.Equals(id)).OrderBy(c => c.RoleId).Select(c => c.RoleId).ToList();
				bpxCache.SetCache(listUserRoleIds, cacheKey, memoryCacheKeyService);
			}
			
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

			foreach (var userRole in listUserRoles)
			{
				userRole.StatusFlag = RecordStatus.Inactive;
				userRole.ModifiedBy = 1;
				userRole.ModifiedDate = DateTime.Now;
			}

			userRoleService.SaveDBChanges();

			foreach (var roleId in roleIds)
			{
				var existingUserRole = userRoleService.GetRecordsByFilter(c => c.UserId == id && c.RoleId == roleId).FirstOrDefault();

				if (existingUserRole != null)
				{
					existingUserRole.StatusFlag = RecordStatus.Active;
					existingUserRole.ModifiedBy = 1;
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

			// remove from cache
			string cacheKey = string.Empty;

			cacheKey = $"user:{id}:roles";
			bpxCache.RemoveCache(cacheKey);

			// set alert
			ShowAlert(AlertType.Success, "User Roles are successfully updated.");

			return RedirectToAction(nameof(Index));
		}

	}
}