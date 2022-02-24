﻿using BPX.Domain.DbModels;
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
		private readonly IPortalService portalService;
		private readonly ILoginService loginService;
		private readonly IUserService userService;
		private readonly IUserRoleService userRoleService;
		private readonly IRolePermitService rolePermitService;
		private readonly IMenuService menuService;
		private readonly IMenuPermitService menuPermitService;

		public CoreService(IConfiguration configuration, ICacheService cacheService, ICacheKeyService cacheKeyService, IPortalService portalService, ILoginService loginService, IUserService userService, IUserRoleService userRoleService, IRolePermitService rolePermitService, IMenuService menuService, IMenuPermitService menuPermitService)
		{
			this.configuration = configuration;
			this.cacheService = cacheService;
			this.cacheKeyService = cacheKeyService;
			this.portalService = portalService;
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

		public IPortalService GetPortalService()
		{
			return portalService;
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

		public List<int> GetUserRoleIds(int userId)
		{
			return userRoleService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.UserId.Equals(userId)).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
		}

		public List<int> GetUserPermitIds(List<int> userRoleIds)
		{
			return rolePermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && userRoleIds.Contains(c.RoleId)).OrderBy(c => c.PermitId).Select(c => c.PermitId).Distinct().ToList();
		}

		public List<int> GetPermitRoles(int permitId)
		{
			return rolePermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && c.PermitId.Equals(permitId)).OrderBy(c => c.RoleId).Select(c => c.RoleId).Distinct().ToList();
		}

		public List<Menu> GetMenuHierarchy(string statusFlag, string orderBy)
		{
			return menuService.GetMenuHierarchy(statusFlag, orderBy);
		}

		public string GetBreadcrumb(int menuId, string currController)
		{
			string breadcrumb = string.Empty;
			List<Menu> listBreadcrumb = menuService.GetBreadCrumb(menuId);

			if (listBreadcrumb != null && listBreadcrumb.Count > 0)
			{
				listBreadcrumb.Reverse();

				foreach (Menu itemBreadcrumb in listBreadcrumb)
				{
					if (itemBreadcrumb.MenuURL.Equals("/"))
					{
						breadcrumb += $"<li class=\"breadcrumb-item\"><a href=\"{itemBreadcrumb.MenuURL}\" class=\"text-decoration-none\"><span class=\"fa fa-home\">&nbsp;</span>{itemBreadcrumb.MenuName}</a></li>";
					}
					else
					{
						if (currController.ToUpper().Equals("HOME"))
						{
							breadcrumb += $"<li class=\"breadcrumb-item\">{itemBreadcrumb.MenuName}</li>";

						}
						else
						{
							breadcrumb += $"<li class=\"breadcrumb-item\"><a href=\"{itemBreadcrumb.MenuURL}\" class=\"text-decoration-none\">{itemBreadcrumb.MenuName}</a></li>";
						}
					}
				}
			}	
			
			return breadcrumb;
		}

		public string GetMenuString(List<int> userPermitIds, List<Menu> menuHierarchy)
		{
			string menuString = string.Empty;
			List<int> listMenuIds = menuPermitService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && userPermitIds.Contains(c.PermitId)).OrderBy(c => c.MenuId).Select(c => c.MenuId).Distinct().ToList();
			List<Menu> listMenu = menuService.GetRecordsByFilter(c => c.StatusFlag.ToUpper().Equals(RecordStatus.Active.ToUpper()) && listMenuIds.Contains(c.MenuId)).OrderBy(c => c.HLevel).ThenBy(c => c.OrderNumber).ToList();

			// generate the menuString
			AddMenuItemsLevelOne(ref menuString, menuHierarchy, AddRoot(ref menuString, menuHierarchy), listMenu, userPermitIds);

			return menuString;
		}

		private Menu AddRoot(ref string menuString, List<Menu> menuHierarchy)
		{
			Menu root = menuHierarchy.Where(c => c.ParentMenuId.Equals(0)).SingleOrDefault();

			//menuString += "<li>";
			//menuString += $"<a class=\"nav-link\" href=\"{root.MenuURL}\">{root.MenuName}</a>";
			//menuString += "</li>";

			return root;
		}

		private void AddMenuItemsLevelOne(ref string menuString, List<Menu> menuHierarchy, Menu menu, List<Menu> listMenu, List<int> userPermitIds)
		{
			List<Menu> listLevelOneMenu = menuHierarchy.Where(c => c.ParentMenuId.Equals(menu.MenuId)).OrderBy(c => c.OrderNumber).ToList();

			if (listLevelOneMenu.Count > 0)
			{
				foreach (Menu itemMenu in listLevelOneMenu)
				{
					bool foundLevelOneItem = false;

					foreach (Menu itemMenu22 in listMenu)
					{
						if (itemMenu22.TreePath.ToUpper().StartsWith(itemMenu.TreePath.ToUpper()))
						{
							foundLevelOneItem = true;
							break;
						}
					}

					if (foundLevelOneItem)
					{
						List<Menu> listLevelTwoMenu = menuHierarchy.Where(c => c.ParentMenuId.Equals(itemMenu.MenuId)).OrderBy(c => c.OrderNumber).ToList();

						if (listLevelTwoMenu.Count > 0)
						{
							menuString += "<li class=\"dropdown\">";
							menuString += $"<a class=\"nav-link\" href=\"{itemMenu.MenuURL}\">{itemMenu.MenuName} <i class=\"fa fa-angle-down\"></i></a>";

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
			List<Menu> listLevelTwoMenu = menuHierarchy.Where(c => c.ParentMenuId.Equals(menu.MenuId)).OrderBy(c => c.OrderNumber).ToList();

			if (listLevelTwoMenu.Count > 0)
			{
				menuString += "<ul>";

				foreach (Menu itemMenu in listLevelTwoMenu)
				{
					bool foundLevelTwoItem = false;

					foreach (Menu itemMenu22 in listMenu)
					{
						if (itemMenu22.TreePath.ToUpper().StartsWith(itemMenu.TreePath.ToUpper()))
						{
							foundLevelTwoItem = true;
							break;
						}
					}

					if (foundLevelTwoItem)
					{
						List<Menu> listLevelThreeMenu = menuHierarchy.Where(c => c.ParentMenuId.Equals(itemMenu.MenuId)).OrderBy(c => c.OrderNumber).ToList();

						if (listLevelThreeMenu.Count > 0)
						{
							menuString += "<li class=\"dropdown\">";
							menuString += $"<a class=\"nav-link\" href=\"{itemMenu.MenuURL}\">{itemMenu.MenuName} <i class=\"fa fa-angle-right\"></i></a>";

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
			List<Menu> listLevelThreeMenu = menuHierarchy.Where(c => c.ParentMenuId.Equals(menu.MenuId)).OrderBy(c => c.OrderNumber).ToList();

			if (listLevelThreeMenu.Count > 0)
			{
				menuString += "<ul>";

				foreach (Menu itemMenu in listLevelThreeMenu)
				{
					bool foundLevelThreeItem = false;

					foreach (Menu itemMenu22 in listMenu)
					{
						if (itemMenu22.TreePath.ToUpper().StartsWith(itemMenu.TreePath.ToUpper()))
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
		IPortalService GetPortalService();
		ILoginService GetLoginService();
		IUserService GetUserService();
		IUserRoleService GetUserRoleService();
		IRolePermitService GetRolePermitService();
		string GetBreadcrumb(int menuId, string currController);
		IMenuService GetMenuService();
		IMenuPermitService GetMenuPermitService();
		List<int> GetUserRoleIds(int userId);
		List<int> GetUserPermitIds(List<int> userRoleIds);
		List<int> GetPermitRoles(int permitId);
		List<Menu> GetMenuHierarchy(string statusFlag, string orderBy);
		string GetMenuString(List<int> userPermitIds, List<Menu> menuHierarchy);
	}
}
