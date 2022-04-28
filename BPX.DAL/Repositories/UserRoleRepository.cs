using BPX.DAL.Context;
using BPX.Domain.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repositories
{
    public class UserRoleRepository : BaseRepository, IUserRoleRepository
    {
        public UserRoleRepository(EFContext efContext, DPContext dpContext) : base(efContext, dpContext)
        {
        }

        public IPagedList<UserRole> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            throw new NotImplementedException();
        }

        public UserRole GetRecordById(int id)
        {
            return efContext.UserRoles.Where(c => c.UserRoleId.Equals(id)).SingleOrDefault();
        }

        public IQueryable<UserRole> GetRecordsByFilter(Expression<Func<UserRole, bool>> filter)
        {
            return efContext.UserRoles.Where(filter);
        }

        public void InsertRecord(UserRole entity)
        {
            efContext.UserRoles.Add(entity);
        }

        public void UpdateRecord(UserRole entity)
        {
            efContext.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IUserRoleRepository : IRepository<UserRole>
    {
    }
}