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
            return errorRepository.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString, filterJson);
        }

        public Error GetRecordById(int id)
        {
            return errorRepository.GetRecordById(id);
        }

        public IQueryable<Error> GetRecordsByFilter(Expression<Func<Error, bool>> filter)
        {
            return errorRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(Error entity)
        {
            //...
            //business rules validation, if any
            //...

            errorRepository.InsertRecord(entity);
        }

        public void UpdateRecord(Error entity)
        {
            //...
            //business rules validation, if any
            //...

            errorRepository.UpdateRecord(entity);
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