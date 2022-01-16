using BPX.DAL.UOW;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class MemoryCacheKeyService : IMemoryCacheKeyService
    {
        public IUnitOfWork _uow;

        public MemoryCacheKeyService(IUnitOfWork uow)
        {
            _uow = uow;
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
            return _uow.MemoryCacheKeyRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(MemoryCacheKey entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.MemoryCacheKeyRepository.InsertRecord(entity);
        }

        public void UpdateRecord(MemoryCacheKey entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.MemoryCacheKeyRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            _uow.SaveDBChanges();
        }
    }

    public interface IMemoryCacheKeyService : IGenericService<MemoryCacheKey>
    {
    }
}