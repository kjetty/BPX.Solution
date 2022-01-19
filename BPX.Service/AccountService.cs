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

			//var watch = new System.Diagnostics.Stopwatch();
			//watch.Start();

			var login = loginService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.UserId.Equals(userId)).SingleOrDefault();
			var user = userService.GetRecordByID(userId);
			var userRolesIds = userRoleService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.UserId == userId).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
			var userPermitIds = rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && userRolesIds.Contains(c.RoleId)).OrderBy(c => c.PermitID).Select(c => c.PermitID).Distinct().ToList();
			
			userMeta.LoginToken = login.LoginToken;
			userMeta.LastLoginDate = (DateTime)login.LastLoginDate;
			userMeta.UserId = login.UserId;
			userMeta.FirstName = user.FirstName;
			userMeta.LastName = user.LastName;
			userMeta.FullName = user.FirstName + " " + user.LastName;
			userMeta.Email = user.Email;
			userMeta.Mobile = user.Mobile;
			userMeta.UserRoleIds = userRolesIds;
			userMeta.UserPermitIds = userPermitIds;

			//watch.Stop();

			//double elapsedTime = (double)watch.ElapsedTicks / (double)Stopwatch.Frequency;
			//string executionTime = (elapsedTime * 1000000).ToString("F2") + " microseconds";

			return userMeta;
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
	}

	public interface IAccountService
	{
		int GetUserId(string loginToken);
		UserMeta GetUserMeta(int userId);
		string GetUserMenuString(List<int> userRoleIds);
	}
}
