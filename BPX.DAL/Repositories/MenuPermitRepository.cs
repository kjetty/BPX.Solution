using BPX.DAL.Context;
using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using BPX.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repository
{
    public class MenuPermitRepository : BaseRepository, IMenuPermitRepository
    {
        public MenuPermitRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<MenuPermit> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public MenuPermit GetRecordByID(int id)
        {
            return context.MenuPermits.Where(c => c.MenuPermitId == id).SingleOrDefault();
        }

        public IQueryable<MenuPermit> GetRecordsByFilter(Expression<Func<MenuPermit, bool>> filter)
        {
            return context.MenuPermits.Where(filter);
        }

        public void InsertRecord(MenuPermit entity)
        {
            context.MenuPermits.Add(entity);
        }

        public void UpdateRecord(MenuPermit entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IMenuPermitRepository : IRepository<MenuPermit>
    {
    }
}