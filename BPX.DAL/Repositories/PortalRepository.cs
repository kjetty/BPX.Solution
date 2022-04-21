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
        public PortalRepository(EFContext efContext) : base(efContext)
        {

        }

        public IPagedList<Portal> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            throw new NotImplementedException();
        }

        public Portal GetRecordById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Portal> GetRecordsByFilter(Expression<Func<Portal, bool>> filter)
        {
            return efContext.Portals.Where(filter);
        }

        public void InsertRecord(Portal entity)
        {
            efContext.Portals.Add(entity);
        }

        public void UpdateRecord(Portal entity)
        {
            efContext.Entry(entity).State = EntityState.Modified;
        }

        public void DetachEntity(Portal entity)
        {
            efContext.Entry(entity).State = EntityState.Detached;
        }
    }

    public interface IPortalRepository : IRepository<Portal>
    {
        void DetachEntity(Portal entity);
    }
}