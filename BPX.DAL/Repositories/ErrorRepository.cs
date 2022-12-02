using BPX.DAL.Context;
using BPX.Domain.DbModels;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repositories
{
    public class ErrorRepository : BaseRepository, IErrorRepository
    {
        public ErrorRepository(EFContext efContext, DPContext dpContext) : base(efContext, dpContext)
        {
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
            throw new NotImplementedException(); ;
        }

        public void InsertRecord(Error entity)
        {
            efContext.Errors.Add(entity);
        }

        public void UpdateRecord(Error entity)
        {
            throw new NotImplementedException();
        }

        // dapper

        public int InsertRecordDapper(Error entity)
        {
            string dynQuery = "insert into Errors (ErrorData, ErrorDate) values (@ErrorData, @ErrorDate)";

            DynamicParameters dynParams = new();
            dynParams.Add("ErrorData", entity.ErrorData);
            dynParams.Add("ErrorDate", entity.ErrorDate);

            using IDbConnection connection = dpContext.CreateConnection();
            int affectedRows = connection.Execute(dynQuery, dynParams);

            return affectedRows;
        }
    }

    public interface IErrorRepository : IRepository<Error>
    {
        int InsertRecordDapper(Error entity);
    }
}