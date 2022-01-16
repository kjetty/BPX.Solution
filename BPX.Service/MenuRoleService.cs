using BPX.DAL.UOW;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class MenuRoleService : IMenuRoleService
    {
        public IUnitOfWork _uow;

        public MenuRoleService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IPagedList<MenuRole> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public MenuRole GetRecordByID(int id)
        {
            return _uow.MenuRoleRepository.GetRecordByID(id);
        }

        public IQueryable<MenuRole> GetRecordsByFilter(Expression<Func<MenuRole, bool>> filter)
        {
            return _uow.MenuRoleRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(MenuRole entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.MenuRoleRepository.InsertRecord(entity);
        }

        public void UpdateRecord(MenuRole entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.MenuRoleRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            _uow.SaveDBChanges();
        }
    }

    public interface IMenuRoleService : IGenericService<MenuRole>
    {
    }
}