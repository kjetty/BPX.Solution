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
            return efContext.Errors.Where(c => c.ErrorId.Equals(id)).SingleOrDefault();
        }

        public IQueryable<Error> GetRecordsByFilter(Expression<Func<Error, bool>> filter)
        {
            return efContext.Errors.Where(filter);
        }

        public void InsertRecord(Error entity)
        {
            efContext.Errors.Add(entity);
        }

        public void UpdateRecord(Error entity)
        {
            efContext.Entry(entity).State = EntityState.Modified;
        }

        public int InsertRecordDapper(Error entity)
        {
            string sqlQuery = "insert into Errors (ErrorData, ErrorDate) values (@ErrorData, @ErrorDate)";

            using IDbConnection connection = dpContext.CreateConnection();
            int affectedRows = connection.Execute(sqlQuery, entity);

            return affectedRows;
        }
    }

    public interface IErrorRepository : IRepository<Error>
    {
        int InsertRecordDapper(Error entity);
    }
}