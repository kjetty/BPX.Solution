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
    public class UserRoleRepository : BaseRepository, IUserRoleRepository
    {
        public UserRoleRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<UserRole> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public UserRole GetRecordByID(int id)
        {
            return context.UserRoles.Where(c => c.UserRoleId == id).SingleOrDefault();
        }

        public IQueryable<UserRole> GetRecordsByFilter(Expression<Func<UserRole, bool>> filter)
        {
            return context.UserRoles.Where(filter);
        }

        public void InsertRecord(UserRole entity)
        {
            context.UserRoles.Add(entity);
        }

        public void UpdateRecord(UserRole entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IUserRoleRepository : IRepository<UserRole>
    {
    }
}