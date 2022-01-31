using BPX.DAL.Repository;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class CacheKeyService : ICacheKeyService
    {
        public CacheKeyRepository CacheKeyRepository;

        public CacheKeyService(ICacheKeyRepository CacheKeyRepository)
        {
            this.CacheKeyRepository = (CacheKeyRepository)CacheKeyRepository;
        }

        public IPagedList<CacheKey> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public CacheKey GetRecordById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<CacheKey> GetRecordsByFilter(Expression<Func<CacheKey, bool>> filter)
        {
            return CacheKeyRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(CacheKey entity)
        {
            //...
            //business rules validation, if any
            //...

            CacheKeyRepository.InsertRecord(entity);
        }

        public void UpdateRecord(CacheKey entity)
        {
            //...
            //business rules validation, if any
            //...

            CacheKeyRepository.UpdateRecord(entity);
        }

		public void SaveDBChanges()
		{
            CacheKeyRepository.SaveDBChanges();
        }
	}

    public interface ICacheKeyService : IGenericService<CacheKey>
    {
    }
}