using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class ErrorService : IErrorService
    {
        public ErrorRepository errorRepository;

        public ErrorService(IErrorRepository errorRepository)
        {
            this.errorRepository = (ErrorRepository)errorRepository;
        }

        public IPagedList<Error> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            throw new NotImplementedException();
        }

        public Error GetRecordById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Error> GetRecordsByFilter(Expression<Func<Error, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public void InsertRecord(Error entity)
        {
            //...
            //business rules validation, if any
            //...

            throw new NotImplementedException();
        }

        public void UpdateRecord(Error entity)
        {
            //...
            //business rules validation, if any
            //...

            throw new NotImplementedException();
        }

        public void SaveDBChanges()
        {
            errorRepository.SaveDBChanges();
        }

        // dapper

        public int InsertRecordDapper(Error entity)
        {
            return errorRepository.InsertRecordDapper(entity);
        }
    }

    public interface IErrorService : IGenericService<Error>
    {
        int InsertRecordDapper(Error entity);
    }
}