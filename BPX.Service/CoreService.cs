using BPX.Domain.CustomModels;
using BPX.Domain.DbModels;
using BPX.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BPX.Service
{
	public class CoreService : ICoreService
	{
		private readonly IConfiguration configuration;
		private readonly ICacheService cacheService; 
		private readonly ICacheKeyService cacheKeyService;
		private readonly ILoginService loginService;
		private readonly IUserService userService;
		private readonly IUserRoleService userRoleService;
		private readonly IRolePermitService rolePermitService;
		private readonly IMenuService menuService;
		private readonly IMenuRoleService menuRoleService;

		public CoreService(IConfiguration configuration, ICacheService cacheService, ICacheKeyService cacheKeyService, ILoginService loginService, IUserService userService, IUserRoleService userRoleService, IRolePermitService rolePermitService, IMenuService menuService, IMenuRoleService menuRoleService)
		{
			this.configuration = configuration;
			this.cacheService = cacheService;
			this.cacheKeyService = cacheKeyService;
			this.loginService = loginService;
			this.userService = userService;
			this.userRoleService = userRoleService;
			this.rolePermitService = rolePermitService;
			this.menuService = menuService;
			this.menuRoleService = menuRoleService;
		}

		public ICacheKeyService GetCacheKeyService()
		{
			return cacheKeyService;
		}

		public ICacheService GetCacheService()
		{
			return cacheService;
		}

		public IConfiguration GetConfiguration()
		{
			return configuration;
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

		public List<int> GetPermitRoles(int permitId)
		{
			return rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.PermitID == permitId).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
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

		public string xxxGenerateMenu()
		{
			string menuString = string.Empty;


			return menuString;


			//var listMenu = new JVKDbContext().Menus.Where(c => c.StatusFlag == "A").ToList();

			////get root or home
			//var root = listMenu.Where(c => c.ParentMenuUID == "the-root").SingleOrDefault();

			////start adding children to the root
			//AddChildMenuItems(listMenu, root);

			//currMenuString = currMenuString.Trim();
			//currMenuString = new Regex("dropdown-menu").Replace(currMenuString, "nav navbar-nav", 1);
			//currMenuString = currMenuString.Trim();

			//return currMenuString;
		}

		private void xxxAddChildMenuItems(List<Menu> listMenu, Menu menu)
		{
			//var listMenuChildren = listMenu.Where(c => c.ParentMenuUID == menu.MenuUID).OrderBy(c => c.OrderNumber).ToList();

			//if (listMenuChildren.Count > 0)
			//{
			//	currMenuString += "<ul class=\"dropdown-menu\">";

			//	foreach (var item in listMenuChildren)
			//	{
			//		currMenuString += "<li>";

			//		//if current item has children
			//		if (listMenu.Where(c => c.ParentMenuUID == item.MenuUID).ToList().Count > 0)
			//		{
			//			if (item.HLevel > 2)
			//				currMenuString += "<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">" + item.MenuName + "<b class=\"caret-right\"></b></a>";
			//			else
			//				currMenuString += "<a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">" + item.MenuName + "<b class=\"caret\"></b></a>";
			//		}
			//		else
			//		{
			//			currMenuString += "<a href=\"" + item.MenuURL + "\">" + item.MenuName + "</a>";
			//		}

			//		//make recursive calls to add children
			//		AddChildMenuItems(listMenu, item);

			//		currMenuString += "</li>";
			//	}

			//	currMenuString += "</ul>";
			//}
		}

	}

	public interface ICoreService
	{
		IConfiguration GetConfiguration();
		ICacheService GetCacheService();
		ICacheKeyService GetCacheKeyService();
		int GetUserId(string loginToken);
		UserMeta GetUserMeta(int userId);
		List<int> GetUserRoleIds(int userId);
		List<int> GetUserPermitIds(List<int> userRoleIds);
		List<int> GetPermitRoles(int permitId);
		string GetUserMenuString(List<int> userRoleIds);
	}
}
