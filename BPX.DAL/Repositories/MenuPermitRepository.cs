using BPX.DAL.Context;
using BPX.Domain.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repositories
{
    public class MenuPermitRepository : BaseRepository, IMenuPermitRepository
    {
        public MenuPermitRepository(EFContext efContext, DPContext dpContext) : base(efContext, dpContext)
        {
        }

        public IPagedList<MenuPermit> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            throw new NotImplementedException();
        }

        public MenuPermit GetRecordById(int id)
        {
            return efContext.MenuPermits.Where(c => c.MenuPermitId.Equals(id)).SingleOrDefault();
        }

        public IQueryable<MenuPermit> GetRecordsByFilter(Expression<Func<MenuPermit, bool>> filter)
        {
            return efContext.MenuPermits.Where(filter);
        }

        public void InsertRecord(MenuPermit entity)
        {
            efContext.MenuPermits.Add(entity);
        }

        public void UpdateRecord(MenuPermit entity)
        {
            efContext.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IMenuPermitRepository : IRepository<MenuPermit>
    {
    }
}