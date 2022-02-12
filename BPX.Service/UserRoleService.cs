using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
	public class UserRoleService : IUserRoleService
    {
        public UserRoleRepository userRoleRepository;

        public UserRoleService(IUserRoleRepository userRoleRepository)
        {
            this.userRoleRepository = (UserRoleRepository)userRoleRepository;
        }

        public IPagedList<UserRole> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public UserRole GetRecordById(int id)
        {
            return userRoleRepository.GetRecordById(id);
        }

        public IQueryable<UserRole> GetRecordsByFilter(Expression<Func<UserRole, bool>> filter)
        {
            return userRoleRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(UserRole entity)
        {
            //...
            //business rules validation, if any
            //...

            userRoleRepository.InsertRecord(entity);
        }

        public void UpdateRecord(UserRole entity)
        {
            //...
            //business rules validation, if any
            //...

            userRoleRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            userRoleRepository.SaveDBChanges();
        }
    }

    public interface IUserRoleService : IGenericService<UserRole>
    {
    }
}