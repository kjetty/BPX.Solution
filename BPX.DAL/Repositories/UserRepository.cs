using BPX.DAL.Context;
using BPX.Domain.DbModels;
using BPX.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;
using Dapper;
using System.Diagnostics;

namespace BPX.DAL.Repositories
{
	public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly DPContext dapperContext;

        public UserRepository(EFContext efContext, DPContext dapperContext) : base(efContext)
        {
            this.dapperContext = dapperContext;
    }

        public IPagedList<User> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
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
            IQueryable<User> model = efContext.Users.Where(c => c.StatusFlag.ToUpper().Equals(statusFlag.ToUpper()));

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
            User user = null;

            Stopwatch watch1 = new System.Diagnostics.Stopwatch();             
            Stopwatch watch2 = new System.Diagnostics.Stopwatch();

            watch1.Start();

            for (int i = 0; i < 1000; i++)
            {
                var abc = efContext.Users.Where(c => c.UserId.Equals(id)).SingleOrDefault();
                //return efContext.Users.Where(c => c.UserId.Equals(id)).SingleOrDefault();
            }

            watch1.Stop();
            string executionTime1 = "[milli: " + watch1.ElapsedMilliseconds.ToString() + " ms]";

            watch2.Start();

            for (int i = 0; i < 1000; i++)
            {
                var query = $"SELECT * FROM Users WHERE 1 = 1 AND UserId = {id}";

                using (var connection = dapperContext.CreateConnection())
                {
                   user = connection.QuerySingleOrDefault<User>(query);

                    //return user;
                }
            }

            watch2.Stop();
            string executionTime2 = "[milli: " + watch2.ElapsedMilliseconds.ToString() + " ms]";

            return user;
        }


        public IQueryable<User> GetRecordsByFilter(Expression<Func<User, bool>> filter)
        {
            return efContext.Users.Where(filter);
        }

        public void InsertRecord(User entity)
        {
            efContext.Users.Add(entity);
        }

        public void UpdateRecord(User entity)
        {
            efContext.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IUserRepository : IRepository<User>
    {
    }
}