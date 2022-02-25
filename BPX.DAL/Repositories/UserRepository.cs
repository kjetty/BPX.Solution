using BPX.DAL.Context;
using BPX.Domain.DbModels;
using BPX.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repositories
{
	public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<User> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            // trim received data
            pageNumber = Convert.ToInt32(pageNumber);
            pageSize = Convert.ToInt32(pageSize);
            statusFlag = statusFlag == null ? string.Empty : statusFlag.Trim();
            sortByColumn = sortByColumn == null ? string.Empty : sortByColumn.Trim();
            sortOrder = sortOrder == null ? string.Empty : sortOrder.Trim();
            searchForString = searchForString == null ? string.Empty : searchForString.Trim();

            // set defaults
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            statusFlag = statusFlag.Length.Equals(0) ? RecordStatus.Active.ToUpper() : statusFlag;
            sortByColumn = sortByColumn.Length.Equals(0) ? "UserId" : sortByColumn;
            sortOrder = sortOrder.Length.Equals(0) ? SortOrder.Ascending.ToUpper() : sortOrder;
            searchForString = searchForString.Length.Equals(0) ? string.Empty : searchForString;

            // get model : IQueryable : apply statusFlag
            IQueryable<User> model = context.Users.Where(c => c.StatusFlag.ToUpper().Equals(statusFlag.ToUpper()));

            // apply search
            if (searchForString.Length > 0)
            {
                model = model.Where(c => c.LastName.ToUpper().Contains(searchForString.ToUpper())
                                || c.FirstName.ToUpper().Contains(searchForString.ToUpper())
                                || c.Email.ToUpper().Contains(searchForString.ToUpper()));
            }

            // apply sort by column, sort order
            switch (sortByColumn.ToUpper())
            {
                case "FIRSTNAME":
                    model = (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.FirstName) : model.OrderBy(c => c.FirstName);
                    break;

                case "LASTNAME":
                    model = (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.LastName) : model.OrderBy(c => c.LastName);
                    break;

                case "EMAIL":
                    model = (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.Email) : model.OrderBy(c => c.Email);
                    break;

                default:
                    model = (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.UserId) : model.OrderBy(c => c.UserId);
                    break;
            }

            // return ToPagedList()
            return model.ToPagedList(pageNumber, pageSize);
        }

        public User GetRecordById(int id)
        {
            return context.Users.Where(c => c.UserId.Equals(id)).SingleOrDefault();
        }

        public IQueryable<User> GetRecordsByFilter(Expression<Func<User, bool>> filter)
        {
            return context.Users.Where(filter);
        }

        public void InsertRecord(User entity)
        {
            context.Users.Add(entity);
        }

        public void UpdateRecord(User entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IUserRepository : IRepository<User>
    {
    }
}