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
    public class RolePermitRepository : BaseRepository, IRolePermitRepository
    {
        public RolePermitRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<RolePermit> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public RolePermit GetRecordByID(int id)
        {
            return _context.RolePermits.Where(c => c.RolePermitID == id).SingleOrDefault();
        }

        public IQueryable<RolePermit> GetRecordsByFilter(Expression<Func<RolePermit, bool>> filter)
        {
            return _context.RolePermits.Where(filter);
        }

        public void InsertRecord(RolePermit entity)
        {
            _context.RolePermits.Add(entity);
        }

        public void UpdateRecord(RolePermit entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IRolePermitRepository : IRepository<RolePermit>
    {
    }
}