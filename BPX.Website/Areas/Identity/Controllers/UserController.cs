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
		public IActionResult Index()
		{
			return RedirectToAction("List");
		}

        // GET: /Identity/User/List
        [Permit(Permits.Identity.User.List)]
        public IActionResult List(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
			// check input and set defaults
			pageNumber = (pageNumber <= 0) ? 1 : pageNumber;
			pageSize = (pageSize <= 0) ? bpxPageSize : pageSize;
			statusFlag = RecordStatus.Active.ToUpper();   //force set to Active records always
			sortByColumn = (sortByColumn == null || sortByColumn.Trim().Length.Equals(0)) ? string.Empty : sortByColumn;
			sortOrder = (sortOrder == null || sortOrder.Trim().Length.Equals(0)) ? SortOrder.Ascending : sortOrder;
			searchForString = (searchForString == null || searchForString.Trim().Length.Equals(0)) ? string.Empty : searchForString;

			// fetch data
			IPagedList<UserMiniViewModel> model = userService.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString).Select(c => (UserMiniViewModel)c);
			List<int> listUserIds = model.Select(c => c.UserId).ToList();
			List<UserRole> listUsersRoles = userRoleService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && listUserIds.Contains(c.UserId)).ToList();
			List<Role> listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).ToList();

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
        public IActionResult Read(int id)
        {
			//todo
			//restrict details view to current user and user with .List permit

			UserViewModel model = (UserViewModel)userService.GetRecordById(id);
			List<UserRole> listUsersRoles = userRoleService.GetRecordsByFilter(c => c.UserId.Equals(id) && c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).ToList();
			List<Role> listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).ToList();
			User modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

			// set ViewBag
			ViewBag.listUsersRoles = listUsersRoles;
			ViewBag.listRoles = listRoles;
			ViewBag.modifiedByName = modelModifiedBy.FirstName + " " + modelModifiedBy.LastName;

			return View(model);
		}

        // GET: /Identity/User/Update/5
        [Permit(Permits.Identity.User.Update)]
        public IActionResult Update(int id)
        {
			UserMiniViewModel model = (UserMiniViewModel)userService.GetRecordById(id);
            List<UserRole> listUsersRoles = userRoleService.GetRecordsByFilter(c => id.Equals(c.UserId) && c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).ToList();
            List<Role> listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).ToList();

			ViewBag.listUsersRoles = listUsersRoles;
			ViewBag.listRoles = listRoles;

			return View(model);
		}

        // POST: /Identity/User/Update/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permit(Permits.Identity.User.Update)]
        public IActionResult Update(int id, UserMiniViewModel collection)
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
                User recordUser = userService.GetRecordById(id);
				
				if (recordUser.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()))
				{
					// set core data
					recordUser.FirstName = collection.FirstName;
					recordUser.LastName = collection.LastName;
					recordUser.Email = collection.Email;
					recordUser.Mobile = collection.Mobile;
					// set generic data
					recordUser.StatusFlag = RecordStatus.Active.ToUpper();
					recordUser.ModifiedBy = currUser.UserId;
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
				logger.Log(LogLevel.Error, ex, "UserController.169");

				// set alert
				ShowAlertBox(AlertType.Error, errorMessage);

				return RedirectToAction(nameof(Update), new { id });
			}
		}

        // GET: /Identity/User/Delete/5
        [Permit(Permits.Identity.User.Delete)]
        public IActionResult Delete(int id)
        {
            UserViewModel model = (UserViewModel)userService.GetRecordById(id);
            List<UserRole> listUsersRoles = userRoleService.GetRecordsByFilter(c => c.UserId.Equals(id) && c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).ToList();
            List<Role> listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).ToList();
			User modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

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
                User recordUser = userService.GetRecordById(id);
				
				if (recordUser.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()))
				{
					// set generic data
					recordUser.StatusFlag = RecordStatus.Inactive.ToUpper();
					recordUser.ModifiedBy = currUser.UserId;
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
        public IActionResult ListDeleted(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
			// check input and set defaults
			pageNumber = (pageNumber <= 0) ? 1 : pageNumber;
			pageSize = (pageSize <= 0) ? bpxPageSize : pageSize;
			statusFlag = RecordStatus.Inactive.ToUpper();   //force set to Active records always
			sortByColumn = (sortByColumn == null || sortByColumn.Trim().Length.Equals(0)) ? string.Empty : sortByColumn;
			sortOrder = (sortOrder == null || sortOrder.Trim().Length.Equals(0)) ? SortOrder.Ascending : sortOrder;
			searchForString = (searchForString == null || searchForString.Trim().Length.Equals(0)) ? string.Empty : searchForString;

            // fetch data
            IPagedList<UserMiniViewModel> model = userService.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString).Select(c => (UserMiniViewModel)c);
            List<int> listUserIds = model.Select(c => c.UserId).ToList();
            List<UserRole> listUsersRoles = userRoleService.GetRecordsByFilter(c => listUserIds.Contains(c.UserId) && c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).ToList();
            List<Role> listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).ToList();

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
        public IActionResult Undelete(int id)
        {
            UserViewModel model = (UserViewModel)userService.GetRecordById(id);
            List<UserRole> listUsersRoles = userRoleService.GetRecordsByFilter(c => c.UserId.Equals(id) && c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).ToList();
            List<Role> listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).ToList();
			User modelModifiedBy = userService.GetRecordById(model.ModifiedBy);

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
                User recordUser = userService.GetRecordById(id);
				
				if (recordUser.StatusFlag.Equals(RecordStatus.Inactive.ToUpper()))
				{
					// set generic data
					recordUser.StatusFlag = RecordStatus.Active.ToUpper();
					recordUser.ModifiedBy = currUser.UserId;
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
				logger.Log(LogLevel.Error, ex, "UserController.358");

				// set alert
				ShowAlertBox(AlertType.Error, errorMessage);

				return RedirectToAction(nameof(Delete), new { id });
			}
		}

		// GET: /Identity/User/Role/5
		[Permit(Permits.Identity.User.UserRoles)]
		public IActionResult UserRoles(int id)
		{
			if (id <= 0)
			{
				// set alert
				ShowAlertBox(AlertType.Error, "User Id is not valid.");

				return RedirectToAction(nameof(Index));
			}

            User user = userService.GetRecordById(id);
            List<Role> listRoles = roleService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper())).OrderBy(c => c.RoleName).ToList();
            List<int> listUserRoleIds = userRoleService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.UserId.Equals(id)).OrderBy(c => c.RoleId).Select(c => c.RoleId).ToList();
			
			// set ViewBag
			ViewBag.user = user;
			ViewBag.listRoles = listRoles;
			ViewBag.listUserRoleIds = listUserRoleIds;

			return View();
		}

		// GET: /Identity/User/Role/5
		[HttpPost]
		[Permit(Permits.Identity.User.UserRoles)]
		public IActionResult UserRoles(int id, List<int> roleIds)
		{
            // get all existing active roles for the user
            List<UserRole> listUserRoles = userRoleService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.UserId.Equals(id)).ToList();

			// delete all existing active roles for the user
			foreach (UserRole userRole in listUserRoles)
			{
				userRole.StatusFlag = RecordStatus.Inactive.ToUpper();
				userRole.ModifiedBy = currUser.UserId;
				userRole.ModifiedDate = DateTime.Now;

				userRoleService.UpdateRecord(userRole);
			}

			userRoleService.SaveDBChanges();

			// add or activate received roles for the user
			foreach (int roleId in roleIds)
			{
                UserRole existingUserRole = userRoleService.GetRecordsByFilter(c => c.UserId.Equals(id) && c.RoleId.Equals(roleId)).SingleOrDefault();

				if (existingUserRole != null)
				{
					existingUserRole.StatusFlag = RecordStatus.Active.ToUpper();
					existingUserRole.ModifiedBy = currUser.UserId;
					existingUserRole.ModifiedDate = DateTime.Now;

					userRoleService.UpdateRecord(existingUserRole);
				}
				else
				{
					UserRole newUserRole = new()
					{
						UserId = id,
						RoleId = roleId,
						StatusFlag = RecordStatus.Active.ToUpper(),
						ModifiedBy = currUser.UserId,
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

            List<string> listCacheKeyNames = cacheKeyService.GetRecordsByFilter(c => c.CacheKeyName.Contains($"user:{id}:") && c.ModifiedDate >= DateTime.Now.AddDays(-999)).OrderBy(c => c.CacheKeyName).Select(c => c.CacheKeyName).ToList();

			foreach (string itemCacheKeyName in listCacheKeyNames)
			{
				cacheService.RemoveCache(itemCacheKeyName.ToString());
			}
		}
	}
}