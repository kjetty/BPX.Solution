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
    public class MenuRepository : BaseRepository, IMenuRepository
    {
        public MenuRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<Menu> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public Menu GetRecordByID(int id)
        {
            return context.Menus.Where(c => c.MenuId == id).SingleOrDefault();
        }

        public IQueryable<Menu> GetRecordsByFilter(Expression<Func<Menu, bool>> filter)
        {
            return context.Menus.Where(filter);
        }

        public void InsertRecord(Menu entity)
        {
            context.Menus.Add(entity);
        }

        public void UpdateRecord(Menu entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IMenuRepository : IRepository<Menu>
    {
    }
}