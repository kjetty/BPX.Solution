using BPX.DAL.Context;
using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repository
{
    public class CacheKeyRepository : BaseRepository, ICacheKeyRepository
    {
        public CacheKeyRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<CacheKey> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public CacheKey GetRecordByID(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<CacheKey> GetRecordsByFilter(Expression<Func<CacheKey, bool>> filter)
        {
            return _context.CacheKeys.Where(filter);
        }

        public void InsertRecord(CacheKey entity)
        {
            _context.CacheKeys.Add(entity);
        }

        public void UpdateRecord(CacheKey entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface ICacheKeyRepository : IRepository<CacheKey>
    {
    }
}