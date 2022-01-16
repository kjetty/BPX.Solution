using BPX.DAL.UOW;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class UserRoleService : IUserRoleService
    {
        public IUnitOfWork _uow;

        public UserRoleService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IPagedList<UserRole> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public UserRole GetRecordByID(int id)
        {
            return _uow.UserRoleRepository.GetRecordByID(id);
        }

        public IQueryable<UserRole> GetRecordsByFilter(Expression<Func<UserRole, bool>> filter)
        {
            return _uow.UserRoleRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(UserRole entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.UserRoleRepository.InsertRecord(entity);
        }

        public void UpdateRecord(UserRole entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.UserRoleRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            _uow.SaveDBChanges();
        }
    }

    public interface IUserRoleService : IGenericService<UserRole>
    {
    }
}