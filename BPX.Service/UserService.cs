using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
	public class UserService : IUserService
    {
		public UserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = (UserRepository)userRepository;
        }

        public IPagedList<User> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            return userRepository.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString);
        }

        public User GetRecordById(int id)
        {
            return userRepository.GetRecordById(id);
        }

        public IQueryable<User> GetRecordsByFilter(Expression<Func<User, bool>> filter)
        {
            return userRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(User entity)
        {
            //...
            //business rules validation, if any
            //...

            userRepository.InsertRecord(entity);
        }

        public void UpdateRecord(User entity)
        {
            //...
            //business rules validation, if any
            //...

            userRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
		{
            userRepository.SaveDBChanges();
        }
	}

    public interface IUserService : IGenericService<User>
    {
    }
}