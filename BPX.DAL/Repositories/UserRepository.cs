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
            pageSize = pageSize <= 0 ? 1 : pageSize;
            statusFlag = statusFlag.Length == 0 ? RecordStatus.Active : statusFlag;
            sortByColumn = sortByColumn.Length == 0 ? "UserId" : sortByColumn;
            sortOrder = sortOrder.Length == 0 ? SortOrder.Ascending : sortOrder;
            searchForString = searchForString.Length == 0 ? string.Empty : searchForString;

            // get model : IQueryable : apply statusFlag
            var model = _context.Users.Where(c => c.StatusFlag == statusFlag);

            // apply search
            if (searchForString.Length > 0)
            {
                model = model.Where(c => c.LastName.ToUpper().Contains(searchForString.Trim().ToUpper())
                                || c.FirstName.ToUpper().Contains(searchForString.Trim().ToUpper())
                                || c.Email.ToUpper().Contains(searchForString.Trim().ToUpper()));
            }

            // apply sort by column, sort order
            switch (sortByColumn)
            {
                case "FirstName":
                    model = (sortOrder == SortOrder.Descending) ? model.OrderByDescending(c => c.FirstName) : model.OrderBy(c => c.FirstName);
                    break;

                case "LastName":
                    model = (sortOrder == SortOrder.Descending) ? model.OrderByDescending(c => c.LastName) : model.OrderBy(c => c.LastName);
                    break;

                case "Email":
                    model = (sortOrder == SortOrder.Descending) ? model.OrderByDescending(c => c.Email) : model.OrderBy(c => c.Email);
                    break;

                default:
                    model = (sortOrder == SortOrder.Descending) ? model.OrderByDescending(c => c.UserId) : model.OrderBy(c => c.UserId);
                    break;
            }

            // return ToPagedList()
            return model.ToPagedList(pageNumber, pageSize);
        }

        public User GetRecordByID(int id)
        {
            return _context.Users.Where(c => c.UserId == id).SingleOrDefault();
        }

        public IQueryable<User> GetRecordsByFilter(Expression<Func<User, bool>> filter)
        {
            return _context.Users.Where(filter);
        }

        public void InsertRecord(User entity)
        {
            _context.Users.Add(entity);
        }

        public void UpdateRecord(User entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IUserRepository : IRepository<User>
    {
    }
}