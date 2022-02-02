using BPX.DAL.Repository;
using BPX.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class MenuService : IMenuService
    {
        public MenuRepository menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            this.menuRepository = (MenuRepository)menuRepository;
        }

        public IPagedList<Menu> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public Menu GetRecordById(int id)
        {
            return menuRepository.GetRecordById(id);
        }

        public IQueryable<Menu> GetRecordsByFilter(Expression<Func<Menu, bool>> filter)
        {
            return menuRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(Menu entity)
        {
            //...
            //business rules validation, if any
            //...

            menuRepository.InsertRecord(entity);
        }

        public void UpdateRecord(Menu entity)
        {
            //...
            //business rules validation, if any
            //...

            menuRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            menuRepository.SaveDBChanges();
        }

        public List<Menu> GetBreadCrumb(int menuId)
		{
            return menuRepository.GetBreadCrumb(menuId);
        }

        public List<Menu> GetMenuHierarchy(string statusflag, string orderBy)
        {
            return menuRepository.GetMenuHierarchy(statusflag, orderBy);
        }
    }

    public interface IMenuService : IGenericService<Menu>
    {
        List<Menu> GetBreadCrumb(int menuId);
        List<Menu> GetMenuHierarchy(string statusflag, string orderBy);
    }
}