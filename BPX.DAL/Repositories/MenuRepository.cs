using BPX.DAL.Context;
using BPX.Domain.DbModels;
using BPX.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repositories
{
	public class MenuRepository : BaseRepository, IMenuRepository
    {
        public MenuRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<Menu> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public Menu GetRecordById(int id)
        {
            return context.Menus.Where(c => c.MenuId.Equals(id)).SingleOrDefault();
        }

        public IQueryable<Menu> GetRecordsByFilter(Expression<Func<Menu, bool>> filter)
        {
            return context.Menus.Where(filter);
        }

        public void InsertRecord(Menu entity)
        {
            context.Menus.Add(entity);
        }

        public void UpdateRecord(Menu entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public List<Menu> GetBreadCrumb(int menuId)
		{
            //WITH CTE_breadcrumb
            //AS
            //(
            //    SELECT      MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, hLevel, OrderNumber, TreePath, StatusFlag, ModifiedBy, ModifiedDate
            //    FROM        Menus
            //    WHERE       UPPER(StatusFlag) = 'A' AND MenuId = 15
            //    UNION ALL
            //    SELECT      m.MenuId, m.MenuName, m.MenuDescription, m.MenuURL, m.ParentMenuId, m.hLevel, m.OrderNumber, m.TreePath, m.StatusFlag, m.ModifiedBy, m.ModifiedDate
            //    FROM        Menus m
            //    INNER JOIN  CTE_breadcrumb cte ON cte.ParentMenuId = m.MenuId
            //    WHERE       UPPER(m.StatusFlag) = 'A'
            //)
            //SELECT    MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, hLevel, OrderNumber, TreePath, StatusFlag, ModifiedBy, ModifiedDate
            //FROM      CTE_breadcrumb

            string cteQuery = string.Empty;
            cteQuery += "WITH CTE_breadcrumb ";
            cteQuery += "AS ";
            cteQuery += "( ";
            cteQuery += "    SELECT      MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, hLevel, OrderNumber, TreePath, StatusFlag, ModifiedBy, ModifiedDate ";
            cteQuery += "    FROM        Menus ";
            cteQuery += "    WHERE       UPPER(StatusFlag) = '" + RecordStatus.Active.ToUpper() + "' AND MenuId = " + Convert.ToInt32(menuId);
            cteQuery += "    UNION ALL ";
            cteQuery += "    SELECT      m.MenuId, m.MenuName, m.MenuDescription, m.MenuURL, m.ParentMenuId, m.hLevel, m.OrderNumber, m.TreePath, m.StatusFlag, m.ModifiedBy, m.ModifiedDate ";
            cteQuery += "    FROM        Menus m ";
            cteQuery += "    INNER JOIN  CTE_breadcrumb cte ON cte.ParentMenuId = m.MenuId ";
            cteQuery += "    WHERE       UPPER(m.StatusFlag) = '" + RecordStatus.Active.ToUpper() + "' ";
            cteQuery += ") ";
            cteQuery += "SELECT    MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, hLevel, OrderNumber, TreePath, StatusFlag, ModifiedBy, ModifiedDate ";
            cteQuery += "FROM      CTE_breadcrumb ";

            return context.Menus.FromSqlRaw(cteQuery).ToList();
        }
    
        public List<Menu> GetMenuHierarchy(string statusFlag, string orderBy)
		{
            ////cte (common time execution) recursive hierarchy query
            //WITH cte_menus AS (
            //    SELECT 	    MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, 1 AS hLevel, OrderNumber, 
            //                  CAST(MenuId AS VARCHAR(32)) AS TreePath, StatusFlag, ModifiedBy, ModifiedDate  
            //    FROM 		    Menus 
            //    WHERE 	    UPPER(StatusFlag) = 'A' 
            //    AND 		    ParentMenuId = 0
            //    UNION ALL
            //    SELECT 	    m.MenuId, m.MenuName, m.MenuDescription, m.MenuURL, m.ParentMenuId, cte.hLevel + 1, m.OrderNumber, 
            //                  CAST(cte.TreePath + '.' + CAST(m.MenuId AS VARCHAR(32)) AS VARCHAR(32)) AS TreePath, m.StatusFlag, m.ModifiedBy, m.ModifiedDate 
            //    FROM 		    Menus m 
            //    INNER JOIN 	cte_menus cte ON cte.MenuId = m.ParentMenuId 
            //    WHERE 	    UPPER(m.StatusFlag) = 'A'
            //)
            //SELECT 	MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, hLevel, OrderNumber, 
            //          CAST('.' + TreePath + '.' AS VARCHAR(32)) AS TreePath, StatusFlag, ModifiedBy, ModifiedDate 
            //FROM 		cte_menus 
            //ORDER BY 	hLevel, OrderNumber

            string cteStatusFlag = RecordStatus.Active.ToUpper();

            if (statusFlag.ToUpper().Equals(RecordStatus.Inactive.ToUpper()))
			{
                cteStatusFlag = RecordStatus.Inactive.ToUpper();
            }

            string cteOrderBy = "hLevel, OrderNumber";

            if (orderBy.ToUpper().Equals("URL"))
			{
                cteOrderBy = "MenuURL";
            }

            string cteQuery = string.Empty;
            cteQuery += "WITH cte_menus AS ( ";
            cteQuery += "	SELECT 		MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, 1 AS hLevel, OrderNumber, ";
            cteQuery += "				CAST(MenuId AS VARCHAR(32)) AS TreePath, StatusFlag, ModifiedBy, ModifiedDate  ";
            cteQuery += "	FROM 		Menus ";
            cteQuery += "	WHERE 		UPPER(StatusFlag) = '" + cteStatusFlag + "'";
            cteQuery += "	AND 		ParentMenuId = 0";
            cteQuery += "	UNION ALL ";
            cteQuery += "	SELECT 		m.MenuId, m.MenuName, m.MenuDescription, m.MenuURL, m.ParentMenuId, cte.hLevel + 1, m.OrderNumber, ";
            cteQuery += "				CAST(cte.TreePath + '.' + CAST(m.MenuId AS VARCHAR(32)) AS VARCHAR(32)) AS TreePath, m.StatusFlag, m.ModifiedBy, m.ModifiedDate ";
            cteQuery += "	FROM 		Menus m ";
            cteQuery += "	INNER JOIN 	cte_menus cte ON cte.MenuId = m.ParentMenuId ";
            cteQuery += "	WHERE 		UPPER(m.StatusFlag) = '" + cteStatusFlag + "'";
            cteQuery += ") ";
            cteQuery += "SELECT 	MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, hLevel, OrderNumber, ";
            cteQuery += "			CAST('.' + TreePath + '.' AS VARCHAR(32)) AS TreePath, StatusFlag, ModifiedBy, ModifiedDate ";
            cteQuery += "FROM 		cte_menus ";
            cteQuery += "ORDER BY " + cteOrderBy;

            List<Menu> listMenu = context.Menus.FromSqlRaw(cteQuery).ToList();

            return listMenu;
        }
    }

    public interface IMenuRepository : IRepository<Menu>
    {
        List<Menu> GetBreadCrumb(int menuId); 
        List<Menu> GetMenuHierarchy(string statusFlag, string orderBy); 
    }
}