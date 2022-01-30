using BPX.DAL.Context;
using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using BPX.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repository
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

        public Menu GetRecordByID(int id)
        {
            return context.Menus.Where(c => c.MenuId == id).SingleOrDefault();
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

        public List<Menu> GetMenuHierarchy(string orderBy)
		{

            ////cte (common time execution) recursive hierarchy query
            //WITH cte_menus AS (
            //    SELECT 	    MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, 1 AS hLevel, OrderNumber, 
            //                  CAST(MenuId AS VARCHAR(32)) AS TreePath, StatusFlag, ModifiedBy, ModifiedDate  
            //    FROM 		    Menus 
            //    WHERE 	    StatusFlag = 'A' 
            //    AND 		    ParentMenuId = 0
            //    UNION ALL
            //    SELECT 	    m.MenuId, m.MenuName, m.MenuDescription, m.MenuURL, m.ParentMenuId, cte.hLevel + 1, m.OrderNumber, 
            //                  CAST(cte.TreePath + '.' + CAST(m.MenuId AS VARCHAR(32)) AS VARCHAR(32)) AS TreePath, m.StatusFlag, m.ModifiedBy, m.ModifiedDate 
            //    FROM 		    Menus m 
            //    INNER JOIN 	cte_menus cte ON cte.MenuId = m.ParentMenuId 
            //    WHERE 	    m.StatusFlag = 'A'
            //)
            //SELECT 	MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, hLevel, OrderNumber, 
            //          CAST('.' + TreePath + '.' AS VARCHAR(32)) AS TreePath, StatusFlag, ModifiedBy, ModifiedDate 
            //FROM 		cte_menus 
            //ORDER BY 	hLevel, OrderNumber

            string cteOrderBy = "hLevel, OrderNumber";

            if (orderBy != null && orderBy.Trim().ToLower().Equals("url"))
			{
                cteOrderBy = "MenuURL";
            }

            string cteQuery = string.Empty;
            cteQuery += "WITH cte_menus AS ( ";
            cteQuery += "	SELECT 		MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, 1 AS hLevel, OrderNumber, ";
            cteQuery += "				CAST(MenuId AS VARCHAR(32)) AS TreePath, StatusFlag, ModifiedBy, ModifiedDate  ";
            cteQuery += "	FROM 		Menus ";
            cteQuery += "	WHERE 		StatusFlag = 'A' ";
            cteQuery += "	AND 		ParentMenuId = 0";
            cteQuery += "	UNION ALL ";
            cteQuery += "	SELECT 		m.MenuId, m.MenuName, m.MenuDescription, m.MenuURL, m.ParentMenuId, cte.hLevel + 1, m.OrderNumber, ";
            cteQuery += "				CAST(cte.TreePath + '.' + CAST(m.MenuId AS VARCHAR(32)) AS VARCHAR(32)) AS TreePath, m.StatusFlag, m.ModifiedBy, m.ModifiedDate ";
            cteQuery += "	FROM 		Menus m ";
            cteQuery += "	INNER JOIN 	cte_menus cte ON cte.MenuId = m.ParentMenuId ";
            cteQuery += "	WHERE 		m.StatusFlag = 'A' ";
            cteQuery += ") ";
            cteQuery += "SELECT 		MenuId, MenuName, MenuDescription, MenuURL, ParentMenuId, hLevel, OrderNumber, ";
            cteQuery += "			CAST('.' + TreePath + '.' AS VARCHAR(32)) AS TreePath, StatusFlag, ModifiedBy, ModifiedDate ";
            cteQuery += "FROM 		cte_menus ";
            cteQuery += "ORDER BY " + cteOrderBy;

            return context.Menus.FromSqlRaw(cteQuery).ToList();
        }
    }

    public interface IMenuRepository : IRepository<Menu>
    {
        List<Menu> GetMenuHierarchy(string orderBy);
    }
}