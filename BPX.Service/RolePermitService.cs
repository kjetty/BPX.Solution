using BPX.DAL.UOW;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class RolePermitService : IRolePermitService
    {
        public IUnitOfWork _uow;

        public RolePermitService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IPagedList<RolePermit> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public RolePermit GetRecordByID(int id)
        {
            return _uow.RolePermitRepository.GetRecordByID(id);
        }

        public IQueryable<RolePermit> GetRecordsByFilter(Expression<Func<RolePermit, bool>> filter)
        {
            return _uow.RolePermitRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(RolePermit entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.RolePermitRepository.InsertRecord(entity);
        }

        public void UpdateRecord(RolePermit entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.RolePermitRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            _uow.SaveDBChanges();
        }
    }

    public interface IRolePermitService : IGenericService<RolePermit>
    {
    }
}