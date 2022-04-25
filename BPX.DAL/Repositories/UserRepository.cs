using BPX.DAL.Context;
using BPX.Domain.DbModels;
using BPX.Domain.FilterModels;
using BPX.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(EFContext efContext, DPContext dpContext) : base(efContext, dpContext)
        {
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
            filterJson = filterJson == null ? string.Empty : filterJson.Trim();

            // set defaults
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            statusFlag = statusFlag.Length.Equals(0) ? RecordStatus.Active.ToUpper() : statusFlag;
            sortByColumn = sortByColumn.Length.Equals(0) ? "UserId" : sortByColumn;
            sortOrder = sortOrder.Length.Equals(0) ? SortOrder.Ascending.ToUpper() : sortOrder;
            searchForString = searchForString.Length.Equals(0) ? string.Empty : searchForString;
            filterJson = filterJson.Length.Equals(0) ? string.Empty : filterJson;

            // get model : IQueryable : apply statusFlag
            IQueryable<User> model = efContext.Users.Where(c => c.StatusFlag.ToUpper().Equals(statusFlag.ToUpper()));

            // generic search
            if (searchForString.Length > 0)
            {
                model = model.Where(c => c.LastName.ToUpper().StartsWith(searchForString.ToUpper())
                                || c.FirstName.ToUpper().StartsWith(searchForString.ToUpper())
                                || c.Email.ToUpper().StartsWith(searchForString.ToUpper()));
            }

            // advanced search using filters
            UserFM userFM = JsonConvert.DeserializeObject<UserFM>(filterJson);

            if (userFM != null)
            {
                if (userFM.FirstName != null)
                    model = model.Where(c => c.FirstName.ToUpper().StartsWith(userFM.FirstName.Trim().ToUpper()));

                if (userFM.LastName != null)
                    model = model.Where(c => c.LastName.ToUpper().StartsWith(userFM.LastName.Trim().ToUpper()));
            }

            // apply sort by column, sort order
            model = sortByColumn.ToUpper() switch
            {
                "FIRSTNAME" => (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.FirstName) : model.OrderBy(c => c.FirstName),
                "LASTNAME" => (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.LastName) : model.OrderBy(c => c.LastName),
                "EMAIL" => (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.Email) : model.OrderBy(c => c.Email),
                _ => (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.UserId) : model.OrderBy(c => c.UserId),
            };

            // return ToPagedList()
            return model.ToPagedList(pageNumber, pageSize);
        }

        public User GetRecordById(int id)
        {
            return efContext.Users.Where(c => c.UserId.Equals(id)).SingleOrDefault();
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