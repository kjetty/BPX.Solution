using BPX.Domain.CustomModels;
using BPX.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPX.Service
{
	public class AccountService : IAccountService
	{
		private readonly ILoginService loginService;
		private readonly IUserService userService;
		private readonly IUserRoleService userRoleService;
		private readonly IRolePermitService rolePermitService;
		private readonly IMenuService menuService;
		private readonly IMenuRoleService menuRoleService;

		public AccountService(ILoginService loginService, IUserService userService, IUserRoleService userRoleService, IRolePermitService rolePermitService, IMenuService menuService, IMenuRoleService menuRoleService)
		{
			this.loginService = loginService;
			this.userService = userService;
			this.userRoleService = userRoleService;
			this.rolePermitService = rolePermitService;
			this.menuService = menuService;
			this.menuRoleService = menuRoleService;
		}

		public int GetUserId(string loginToken)
		{
			var login = loginService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.LoginToken.Equals(loginToken)).SingleOrDefault();

			if (login != null)
			{
				return login.UserId;
			}

			return 0;
		}
		
		public UserMeta GetUserMeta(int userId)
		{
			UserMeta userMeta = new UserMeta();


			var user = userService.GetRecordByID(userId);
			
			userMeta.UserId = userId;
			userMeta.FirstName = user.FirstName;
			userMeta.LastName = user.LastName;
			userMeta.FullName = user.FirstName + " " + user.LastName;
			userMeta.Email = user.Email;
			userMeta.Mobile = user.Mobile;



			return userMeta;
		}

		public List<int> GetUserRoleIds(int userId)
		{
			return userRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.UserId == userId).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
		}

		public List<int> GetUserPermitIds(List<int> userRoleIds)
		{
			return rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && userRoleIds.Contains(c.RoleId)).OrderBy(c => c.PermitID).Select(c => c.PermitID).Distinct().ToList();
		}

		public string GetUserMenuString(List<int> userRoleIds)
		{
			var menuRoleList = menuRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals("A") && userRoleIds.Contains(c.RoleId)).Select(c => c.MenuId).ToList();
			var menuList = menuService.GetRecordsByFilter(c => c.StatusFlag.Equals("A")).ToList();

			string menuString = string.Empty;

			foreach (var itemMenu in menuList)
			{
				if (menuRoleList.Contains(itemMenu.MenuId))
				{
					menuString += $"<li class=\"nav-item\"><a class=\"nav-link text-dark\" href=\"{itemMenu.MenuURL}\">{itemMenu.MenuName}</a></li>";
				}
			}

			return menuString;
		}

		public bool IsUserPermitted(string logintoken, int permitId)
		{
			// this call for permit validation is not recommended, because these database calls cannot be cached (not here)
			bool retVal = false;

			int userId = GetUserId(logintoken);
			var userRolesIds = userRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.UserId == userId).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
			var permitRoleIds = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && userRolesIds.Contains(c.RoleId) && c.PermitID == permitId).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();

			if (permitRoleIds.Count > 0)
			{
				retVal = true;
			}

			return retVal;
		}

	}

	public interface IAccountService
	{
		int GetUserId(string loginToken);
		UserMeta GetUserMeta(int userId);
		List<int> GetUserRoleIds(int userId);
		List<int> GetUserPermitIds(List<int> userRoleIds);
		string GetUserMenuString(List<int> userRoleIds);
		bool IsUserPermitted(string logintoken, int permitId);
	}
}
