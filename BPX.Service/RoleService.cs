using BPX.DAL.UOW;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class RoleService : IRoleService
    {
        public IUnitOfWork _uow;

        public RoleService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IPagedList<Role> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            return _uow.RoleRepository.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString);
        }

        public Role GetRecordByID(int id)
        {
            return _uow.RoleRepository.GetRecordByID(id);
        }

        public IQueryable<Role> GetRecordsByFilter(Expression<Func<Role, bool>> filter)
        {
            return _uow.RoleRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(Role entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.RoleRepository.InsertRecord(entity);
        }

        public void UpdateRecord(Role entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.RoleRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            _uow.SaveDBChanges();
        }
    }

    public interface IRoleService : IGenericService<Role>
    {
    }
}