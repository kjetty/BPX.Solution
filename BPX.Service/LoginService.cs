using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class LoginService : ILoginService
    {
        public LoginRepository loginRepository;

        public LoginService(ILoginRepository loginRepository)
        {
            this.loginRepository = (LoginRepository)loginRepository;
        }

        public IPagedList<Login> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            throw new NotImplementedException();
        }

        public Login GetRecordById(int id)
        {
            return loginRepository.GetRecordById(id);
        }

        public IQueryable<Login> GetRecordsByFilter(Expression<Func<Login, bool>> filter)
        {
            return loginRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(Login entity)
        {
            //...
            //business rules validation, if any
            //...

            loginRepository.InsertRecord(entity);
        }

        public void UpdateRecord(Login entity)
        {
            //...
            //business rules validation, if any
            //...

            loginRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            loginRepository.SaveDBChanges();
        }

        // dapper

        public Login GetLoginByToken(string lToken)
        {
            return loginRepository.GetLoginByToken(lToken);
        }
    }

    public interface ILoginService : IGenericService<Login>
    {
        Login GetLoginByToken(string lToken);
    }
}