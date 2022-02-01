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
		private readonly IMenuPermitService menuPermitService;

		public CoreService(IConfiguration configuration, ICacheService cacheService, ICacheKeyService cacheKeyService, ILoginService loginService, IUserService userService, IUserRoleService userRoleService, IRolePermitService rolePermitService, IMenuService menuService, IMenuPermitService menuPermitService)
		{
			this.configuration = configuration;
			this.cacheService = cacheService;
			this.cacheKeyService = cacheKeyService;
			this.loginService = loginService;
			this.userService = userService;
			this.userRoleService = userRoleService;
			this.rolePermitService = rolePermitService;
			this.menuService = menuService;
			this.menuPermitService = menuPermitService;
		}
		
		public IConfiguration GetConfiguration()
		{
			return configuration;
		}

		public ICacheService GetCacheService()
		{
			return cacheService;
		}

		public ICacheKeyService GetCacheKeyService()
		{
			return cacheKeyService;
		}

		public ILoginService GetLoginService()
		{
			return loginService;
		}
		
		public IUserService GetUserService()
		{
			return userService;
		}
		
		public IUserRoleService GetUserRoleService()
		{
			return userRoleService;
		}
		
		public IRolePermitService GetRolePermitService()
		{
			return rolePermitService;
		}
		
		public IMenuService GetMenuService()
		{
			return menuService;
		}
		
		public IMenuPermitService GetMenuPermitService()
		{
			return menuPermitService;
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

			var user = userService.GetRecordById(userId);

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
			return rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && userRoleIds.Contains(c.RoleId)).OrderBy(c => c.PermitId).Select(c => c.PermitId).Distinct().ToList();
		}

		public List<int> GetPermitRoles(int permitId)
		{
			return rolePermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && c.PermitId == permitId).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
		}

		public List<Menu> GetMenuHierarchy(string statusFlag, string orderBy)
		{
			return menuService.GetMenuHierarchy(statusFlag, orderBy);
		}

		public string GetMenuString(List<int> userPermitIds, List<Menu> menuHierarchy)
		{
			string menuString = string.Empty;
			List<int> listMenuIds = menuPermitService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && userPermitIds.Contains(c.PermitId)).OrderBy(c => c.MenuId).Select(c => c.MenuId).Distinct().ToList();
			List<Menu> listMenu = menuService.GetRecordsByFilter(c => c.StatusFlag.Equals(RecordStatus.Active) && listMenuIds.Contains(c.MenuId)).OrderBy(c => c.MenuURL).ToList();

			// generate the menuString
			AddMenuItemsLevelOne(ref menuString, menuHierarchy, AddRoot(ref menuString, menuHierarchy), listMenu, userPermitIds);

			return menuString;
		}

		private Menu AddRoot(ref string menuString, List<Menu> menuHierarchy)
		{
			var root = menuHierarchy.Where(c => c.ParentMenuId.Equals(0)).SingleOrDefault();

			//menuString += "<li>";
			//menuString += $"<a class=\"nav-link\" href=\"{root.MenuURL}\">{root.MenuName}</a>";
			//menuString += "</li>";

			return root;
		}

		private void AddMenuItemsLevelOne(ref string menuString, List<Menu> menuHierarchy, Menu menu, List<Menu> listMenu, List<int> userPermitIds)
		{
			var levelOneMenuItems = menuHierarchy.Where(c => c.ParentMenuId.Equals(menu.MenuId)).ToList();

			if (levelOneMenuItems.Count > 0)
			{
				foreach (var itemMenu in levelOneMenuItems)
				{
					bool foundLevelOneItem = false;

					foreach (var itemMenu22 in listMenu)
					{
						if (itemMenu22.TreePath.StartsWith(itemMenu.TreePath))
						{
							foundLevelOneItem = true;
							break;
						}
					}

					if (foundLevelOneItem)
					{
						var levelTwoMenuItems = menuHierarchy.Where(c => c.ParentMenuId.Equals(itemMenu.MenuId)).ToList();

						if (levelTwoMenuItems.Count > 0)
						{
							menuString += "<li class=\"dropdown\">";
							menuString += $"<a class=\"nav-link\" href=\"{itemMenu.MenuURL}\">{itemMenu.MenuName} <i class=\"bi bi-chevron-down\"></i></a>";

							AddMenuItemsLevelTwo(ref menuString, menuHierarchy, itemMenu, listMenu, userPermitIds);

							menuString += "</li>";
						}
						else
						{
							menuString += "<li>";
							menuString += $"<a class=\"nav-link\" href=\"{itemMenu.MenuURL}\">{itemMenu.MenuName}</a>";
							menuString += "</li>";
						}
					}
				}

			}
		}

		private void AddMenuItemsLevelTwo(ref string menuString, List<Menu> menuHierarchy, Menu menu, List<Menu> listMenu, List<int> userPermitIds)
		{
			var levelTwoMenuItems = menuHierarchy.Where(c => c.ParentMenuId.Equals(menu.MenuId)).ToList();

			if (levelTwoMenuItems.Count > 0)
			{
				menuString += "<ul>";

				foreach (var itemMenu in levelTwoMenuItems)
				{
					bool foundLevelTwoItem = false;

					foreach (var itemMenu22 in listMenu)
					{
						if (itemMenu22.TreePath.StartsWith(itemMenu.TreePath))
						{
							foundLevelTwoItem = true;
							break;
						}
					}

					if (foundLevelTwoItem)
					{
						var levelThreeMenuItems = menuHierarchy.Where(c => c.ParentMenuId.Equals(itemMenu.MenuId)).ToList();

						if (levelThreeMenuItems.Count > 0)
						{
							menuString += "<li class=\"dropdown\">";
							menuString += $"<a class=\"nav-link\" href=\"{itemMenu.MenuURL}\">{itemMenu.MenuName} <i class=\"bi bi-chevron-right\"></i></a>";

							AddMenuItemsLevelThree(ref menuString, menuHierarchy, itemMenu, listMenu, userPermitIds);

							menuString += "</li>";
						}
						else
						{
							menuString += "<li>";
							menuString += $"<a class=\"nav-link\" href=\"{itemMenu.MenuURL}\">{itemMenu.MenuName}</a>";
							menuString += "</li>";
						}
					}
				}

				menuString += "</ul>";
			}
		}

		private void AddMenuItemsLevelThree(ref string menuString, List<Menu> menuHierarchy, Menu menu, List<Menu> listMenu, List<int> userPermitIds)
		{
			var levelThreeMenuItems = menuHierarchy.Where(c => c.ParentMenuId.Equals(menu.MenuId)).ToList();

			if (levelThreeMenuItems.Count > 0)
			{
				menuString += "<ul>";

				foreach (var itemMenu in levelThreeMenuItems)
				{
					bool foundLevelThreeItem = false;

					foreach (var itemMenu22 in listMenu)
					{
						if (itemMenu22.TreePath.StartsWith(itemMenu.TreePath))
						{
							foundLevelThreeItem = true;
							break;
						}
					}					
					
					if (foundLevelThreeItem)
					{							
						menuString += "<li>";
						menuString += $"<a class=\"nav-link\" href=\"{itemMenu.MenuURL}\">{itemMenu.MenuName}</a>";
						menuString += "</li>";
					}
				}

				menuString += "</ul>";
			}
		}
	}

	public interface ICoreService
	{
		IConfiguration GetConfiguration();
		ICacheService GetCacheService();
		ICacheKeyService GetCacheKeyService();
		ILoginService GetLoginService();
		IUserService GetUserService();
		IUserRoleService GetUserRoleService();
		IRolePermitService GetRolePermitService();
		IMenuService GetMenuService();
		IMenuPermitService GetMenuPermitService();
		int GetUserId(string loginToken);
		UserMeta GetUserMeta(int userId);
		List<int> GetUserRoleIds(int userId);
		List<int> GetUserPermitIds(List<int> userRoleIds);
		List<int> GetPermitRoles(int permitId);
		List<Menu> GetMenuHierarchy(string statusFlag, string orderBy);
		string GetMenuString(List<int> userPermitIds, List<Menu> menuHierarchy);
	}
}
