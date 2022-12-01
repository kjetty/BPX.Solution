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
    public class CacheKeyRepository : BaseRepository, ICacheKeyRepository
    {
        public CacheKeyRepository(EFContext efContext, DPContext dpContext) : base(efContext, dpContext)
        {
        }

        public IPagedList<CacheKey> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            throw new NotImplementedException();
        }

        public CacheKey GetRecordById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<CacheKey> GetRecordsByFilter(Expression<Func<CacheKey, bool>> filter)
        {
            return efContext.CacheKeys.Where(filter);
        }

        public void InsertRecord(CacheKey entity)
        {
            efContext.CacheKeys.Add(entity);
        }

        public void UpdateRecord(CacheKey entity)
        {
            efContext.Entry(entity).State = EntityState.Modified;
        }

        // dapper

        public void TruncateTableCacheKeysDapper()
        {
            string dynQuery = "truncate table CacheKeys";
             
            using IDbConnection connection = dpContext.CreateConnection();
            int affectedRows = connection.Execute(dynQuery);
        }
    }

    public interface ICacheKeyRepository : IRepository<CacheKey>
    {
        void TruncateTableCacheKeysDapper();
    }
}