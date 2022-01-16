using BPX.DAL.UOW;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class PermitService : IPermitService
    {
        public IUnitOfWork _uow;

        public PermitService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IPagedList<Permit> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            return _uow.PermitRepository.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString);
        }

        public Permit GetRecordByID(int id)
        {
            return _uow.PermitRepository.GetRecordByID(id);
        }

        public IQueryable<Permit> GetRecordsByFilter(Expression<Func<Permit, bool>> filter)
        {
            return _uow.PermitRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(Permit entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.PermitRepository.InsertRecord(entity);
        }

        public void UpdateRecord(Permit entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.PermitRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            _uow.SaveDBChanges();
        }
    }

    public interface IPermitService : IGenericService<Permit>
    {
    }
}