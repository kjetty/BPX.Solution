using BPX.DAL.Repository;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class MemoryCacheKeyService : IMemoryCacheKeyService
    {
        public MemoryCacheKeyRepository memoryCacheKeyRepository;

        public MemoryCacheKeyService(IMemoryCacheKeyRepository memoryCacheKeyRepository)
        {
            this.memoryCacheKeyRepository = (MemoryCacheKeyRepository)memoryCacheKeyRepository;
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
            return memoryCacheKeyRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(MemoryCacheKey entity)
        {
            //...
            //business rules validation, if any
            //...

            memoryCacheKeyRepository.InsertRecord(entity);
        }

        public void UpdateRecord(MemoryCacheKey entity)
        {
            //...
            //business rules validation, if any
            //...

            memoryCacheKeyRepository.UpdateRecord(entity);
        }

		public void SaveDBChanges()
		{
            memoryCacheKeyRepository.SaveDBChanges();
        }
	}

    public interface IMemoryCacheKeyService : IGenericService<MemoryCacheKey>
    {
    }
}