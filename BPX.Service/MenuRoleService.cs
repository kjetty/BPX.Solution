using BPX.DAL.Repository;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class MenuRoleService : IMenuRoleService
    {
        public MenuRoleRepository menuRoleRepository;

        public MenuRoleService(IMenuRoleRepository menuRoleRepository)
        {
            this.menuRoleRepository = (MenuRoleRepository)menuRoleRepository;
        }

        public IPagedList<MenuRole> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public MenuRole GetRecordByID(int id)
        {
            return menuRoleRepository.GetRecordByID(id);
        }

        public IQueryable<MenuRole> GetRecordsByFilter(Expression<Func<MenuRole, bool>> filter)
        {
            return menuRoleRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(MenuRole entity)
        {
            //...
            //business rules validation, if any
            //...

            menuRoleRepository.InsertRecord(entity);
        }

        public void UpdateRecord(MenuRole entity)
        {
            //...
            //business rules validation, if any
            //...

            menuRoleRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            menuRoleRepository.SaveDBChanges();
        }
    }

    public interface IMenuRoleService : IGenericService<MenuRole>
    {
    }
}