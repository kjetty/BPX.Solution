﻿using BPX.Domain.CustomModels;
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
		private readonly IMenuPermitService menuRoleService;
		private string menuString;

		public CoreService(IConfiguration configuration, ICacheService cacheService, ICacheKeyService cacheKeyService, ILoginService loginService, IUserService userService, IUserRoleService userRoleService, IRolePermitService rolePermitService, IMenuService menuService, IMenuPermitService menuRoleService)
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
			this.menuString = string.Empty;
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

		public List<Menu> GetMenuAll()
		{
			return menuService.GetMenuHierarchy();
		}	

		public string GetMenuString(List<int> userPermitIds, List<Menu> menuAll)
		{
			menuString = string.Empty;

			// todo :: get TreePath(s) from userPermitIds



			// generate the menuString
			AddMenuItemsLevelOne(menuAll, AddRoot(menuAll));

			return menuString;
		}

		private Menu AddRoot(List<Menu> menuAll)
		{
			var root = menuAll.Where(c => c.ParentMenuId.Equals(0)).SingleOrDefault();

			menuString += "<li>";
			menuString += $"<a class=\"nav-link\" href=\"{root.MenuURL}\">{root.MenuName}</a>";
			menuString += "</li>";

			return root;
		}

		private void AddMenuItemsLevelOne(List<Menu> menuAll, Menu menu)
		{
			var levelOneMenuItems = menuAll.Where(c => c.ParentMenuId.Equals(menu.MenuId)).ToList();

			if (levelOneMenuItems.Count > 0)
			{
				foreach (var itemMenu in levelOneMenuItems)
				{
					//string allowedTreePath = ".1.3.17.45.";

					//if (allowedTreePath.StartsWith(itemMenu.TreePath))
					{
						var levelTwoMenuItems = menuAll.Where(c => c.ParentMenuId.Equals(itemMenu.MenuId)).ToList();

						if (levelTwoMenuItems.Count > 0)
						{
							menuString += "<li class=\"dropdown\">";
							menuString += $"<a class=\"nav-link\" href=\"{itemMenu.MenuURL}\">{itemMenu.MenuName} <i class=\"bi bi-chevron-down\"></i></a>";

							AddMenuItemsLevelTwo(menuAll, itemMenu);

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

		private void AddMenuItemsLevelTwo(List<Menu> menuAll, Menu menu)
		{
			var levelTwoMenuItems = menuAll.Where(c => c.ParentMenuId.Equals(menu.MenuId)).ToList();

			if (levelTwoMenuItems.Count > 0)
			{
				menuString += "<ul>";

				foreach (var itemMenu in levelTwoMenuItems)
				{
					//string allowedTreePath = ".1.3.17.45.";

					//if (allowedTreePath.StartsWith(itemMenu.TreePath))
					{
						var levelThreeMenuItems = menuAll.Where(c => c.ParentMenuId.Equals(itemMenu.MenuId)).ToList();

						if (levelThreeMenuItems.Count > 0)
						{
							menuString += "<li class=\"dropdown\">";
							menuString += $"<a class=\"nav-link\" href=\"{itemMenu.MenuURL}\">{itemMenu.MenuName} <i class=\"bi bi-chevron-right\"></i></a>";

							AddMenuItemsLevelThree(menuAll, itemMenu);

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

		private void AddMenuItemsLevelThree(List<Menu> menuAll, Menu menu)
		{
			var levelThreeMenuItems = menuAll.Where(c => c.ParentMenuId.Equals(menu.MenuId)).ToList();

			if (levelThreeMenuItems.Count > 0)
			{
				menuString += "<ul>";

				foreach (var itemMenu in levelThreeMenuItems)
				{
					//string allowedTreePath = ".1.3.17.45.";

					//if (allowedTreePath.StartsWith(itemMenu.TreePath))
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
		int GetUserId(string loginToken);
		UserMeta GetUserMeta(int userId);
		List<int> GetUserRoleIds(int userId);
		List<int> GetUserPermitIds(List<int> userRoleIds);
		List<int> GetPermitRoles(int permitId);
		List<Menu> GetMenuAll();
		string GetMenuString(List<int> userPermitIds, List<Menu> menuAll);
	}
}
