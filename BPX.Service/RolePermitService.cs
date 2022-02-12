using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
	public class RolePermitService : IRolePermitService
    {
        public RolePermitRepository rolePermitRepository;

        public RolePermitService(IRolePermitRepository rolePermitRepository)
        {
            this.rolePermitRepository = (RolePermitRepository)rolePermitRepository;
        }

        public IPagedList<RolePermit> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public RolePermit GetRecordById(int id)
        {
            return rolePermitRepository.GetRecordById(id);
        }

        public IQueryable<RolePermit> GetRecordsByFilter(Expression<Func<RolePermit, bool>> filter)
        {
            return rolePermitRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(RolePermit entity)
        {
            //...
            //business rules validation, if any
            //...

            rolePermitRepository.InsertRecord(entity);
        }

        public void UpdateRecord(RolePermit entity)
        {
            //...
            //business rules validation, if any
            //...

            rolePermitRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            rolePermitRepository.SaveDBChanges();
        }
    }

    public interface IRolePermitService : IGenericService<RolePermit>
    {
    }
}