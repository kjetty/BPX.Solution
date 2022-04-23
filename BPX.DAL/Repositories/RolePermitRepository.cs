using BPX.DAL.Context;
using BPX.Domain.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repositories
{
	public class RolePermitRepository : BaseRepository, IRolePermitRepository
    {
        public RolePermitRepository(EFContext efContext, DPContext dpContext) : base(efContext, dpContext)
        {
        }

        public IPagedList<RolePermit> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            throw new NotImplementedException();
        }

        public RolePermit GetRecordById(int id)
        {
            return efContext.RolePermits.Where(c => c.RolePermitId.Equals(id)).SingleOrDefault();
        }

        public IQueryable<RolePermit> GetRecordsByFilter(Expression<Func<RolePermit, bool>> filter)
        {
            return efContext.RolePermits.Where(filter);
        }

        public void InsertRecord(RolePermit entity)
        {
            efContext.RolePermits.Add(entity);
        }

        public void UpdateRecord(RolePermit entity)
        {
            efContext.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IRolePermitRepository : IRepository<RolePermit>
    {
    }
}