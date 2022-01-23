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
    public class MenuRoleRepository : BaseRepository, IMenuRoleRepository
    {
        public MenuRoleRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<MenuRole> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public MenuRole GetRecordByID(int id)
        {
            return context.MenuRoles.Where(c => c.MenuRoleId == id).SingleOrDefault();
        }

        public IQueryable<MenuRole> GetRecordsByFilter(Expression<Func<MenuRole, bool>> filter)
        {
            return context.MenuRoles.Where(filter);
        }

        public void InsertRecord(MenuRole entity)
        {
            context.MenuRoles.Add(entity);
        }

        public void UpdateRecord(MenuRole entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IMenuRoleRepository : IRepository<MenuRole>
    {
    }
}