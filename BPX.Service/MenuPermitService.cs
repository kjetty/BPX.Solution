using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
	public class MenuPermitService : IMenuPermitService
    {
        public MenuPermitRepository menuPermitRepository;

        public MenuPermitService(IMenuPermitRepository menuPermitRepository)
        {
            this.menuPermitRepository = (MenuPermitRepository)menuPermitRepository;
        }

        public IPagedList<MenuPermit> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public MenuPermit GetRecordById(int id)
        {
            return menuPermitRepository.GetRecordById(id);
        }

        public IQueryable<MenuPermit> GetRecordsByFilter(Expression<Func<MenuPermit, bool>> filter)
        {
            return menuPermitRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(MenuPermit entity)
        {
            //...
            //business rules validation, if any
            //...

            menuPermitRepository.InsertRecord(entity);
        }

        public void UpdateRecord(MenuPermit entity)
        {
            //...
            //business rules validation, if any
            //...

            menuPermitRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            menuPermitRepository.SaveDBChanges();
        }
    }

    public interface IMenuPermitService : IGenericService<MenuPermit>
    {
    }
}