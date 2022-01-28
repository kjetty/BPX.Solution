using BPX.DAL.Repository;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class MenuPermitService : IMenuPermitService
    {
        public MenuPermitRepository menuRoleRepository;

        public MenuPermitService(IMenuPermitRepository menuRoleRepository)
        {
            this.menuRoleRepository = (MenuPermitRepository)menuRoleRepository;
        }

        public IPagedList<MenuPermit> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public MenuPermit GetRecordByID(int id)
        {
            return menuRoleRepository.GetRecordByID(id);
        }

        public IQueryable<MenuPermit> GetRecordsByFilter(Expression<Func<MenuPermit, bool>> filter)
        {
            return menuRoleRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(MenuPermit entity)
        {
            //...
            //business rules validation, if any
            //...

            menuRoleRepository.InsertRecord(entity);
        }

        public void UpdateRecord(MenuPermit entity)
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

    public interface IMenuPermitService : IGenericService<MenuPermit>
    {
    }
}