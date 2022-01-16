using BPX.DAL.UOW;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class LoginService : ILoginService
    {
        public IUnitOfWork _uow;

        public LoginService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IPagedList<Login> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public Login GetRecordByID(int id)
        {
            return _uow.LoginRepository.GetRecordByID(id);
        }

        public IQueryable<Login> GetRecordsByFilter(Expression<Func<Login, bool>> filter)
        {
            return _uow.LoginRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(Login entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.LoginRepository.InsertRecord(entity);
        }

        public void UpdateRecord(Login entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.LoginRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            _uow.SaveDBChanges();
        }
    }

    public interface ILoginService : IGenericService<Login>
    {
    }
}