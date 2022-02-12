using BPX.DAL.Context;
using BPX.Domain.DbModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repositories
{
	public class LoginRepository : BaseRepository, ILoginRepository
    {
        public LoginRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<Login> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public Login GetRecordById(int id)
        {
            return context.Logins.Where(c => c.UserId.Equals(id)).SingleOrDefault();
        }

        public IQueryable<Login> GetRecordsByFilter(Expression<Func<Login, bool>> filter)
        {
            return context.Logins.Where(filter);
        }

        public void InsertRecord(Login entity)
        {
            context.Logins.Add(entity);
        }

        public void UpdateRecord(Login entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface ILoginRepository : IRepository<Login>
    {
    }
}