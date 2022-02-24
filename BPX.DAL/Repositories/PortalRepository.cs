using BPX.DAL.Context;
using BPX.Domain.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repositories
{
	public class PortalRepository : BaseRepository, IPortalRepository
    {
        public PortalRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<Portal> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public Portal GetRecordById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Portal> GetRecordsByFilter(Expression<Func<Portal, bool>> filter)
        {
            return context.Portals.Where(filter);
        }

        public void InsertRecord(Portal entity)
        {
            context.Portals.Add(entity);
        }

        public void UpdateRecord(Portal entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IPortalRepository : IRepository<Portal>
    {
    }
}