using BPX.DAL.UOW;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class UserService : IUserService
    {
		public IUnitOfWork _uow;

        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IPagedList<User> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            return _uow.UserRepository.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString);
        }

        public User GetRecordByID(int id)
        {
            return _uow.UserRepository.GetRecordByID(id);
        }

        public IQueryable<User> GetRecordsByFilter(Expression<Func<User, bool>> filter)
        {
            return _uow.UserRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(User entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.UserRepository.InsertRecord(entity);
        }

        public void UpdateRecord(User entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.UserRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            _uow.SaveDBChanges();
        }
    }

    public interface IUserService : IGenericService<User>
    {
    }
}