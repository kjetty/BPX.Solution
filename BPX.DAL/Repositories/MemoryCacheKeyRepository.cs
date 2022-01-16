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
    public class MemoryCacheKeyRepository : BaseRepository, IMemoryCacheKeyRepository
    {
        public MemoryCacheKeyRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<MemoryCacheKey> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public MemoryCacheKey GetRecordByID(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<MemoryCacheKey> GetRecordsByFilter(Expression<Func<MemoryCacheKey, bool>> filter)
        {
            return _context.MemoryCacheKeys.Where(filter);
        }

        public void InsertRecord(MemoryCacheKey entity)
        {
            _context.MemoryCacheKeys.Add(entity);
        }

        public void UpdateRecord(MemoryCacheKey entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IMemoryCacheKeyRepository : IRepository<MemoryCacheKey>
    {
    }
}