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
	public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<Role> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
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
            statusFlag = statusFlag.Length.Equals(0) ? RecordStatus.Active : statusFlag;
            sortByColumn = sortByColumn.Length.Equals(0) ? "RoleId" : sortByColumn;
            sortOrder = sortOrder.Length.Equals(0) ? SortOrder.Ascending : sortOrder;
            searchForString = searchForString.Length.Equals(0) ? string.Empty : searchForString;

            // get model : IQueryable : apply statusFlag
            var model = context.Roles.Where(c => c.StatusFlag.ToUpper().Equals(statusFlag.ToUpper()));

            // apply search
            if (searchForString.Length > 0)
            {
                model = model.Where(c => c.RoleName.ToUpper().Contains(searchForString.ToUpper())
                                || c.RoleDescription.ToUpper().Contains(searchForString.ToUpper()));
            }

            // apply sort by column, sort order
            switch (sortByColumn.ToUpper())
            {
                case "FIRSTNAME":
                    model = (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.RoleName) : model.OrderBy(c => c.RoleName);
                    break;

                case "LASTNAME":
                    model = (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.RoleDescription) : model.OrderBy(c => c.RoleDescription);
                    break;

                default:
                    model = (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.RoleId) : model.OrderBy(c => c.RoleId);
                    break;
            }

            // return ToPagedList()
            return model.ToPagedList(pageNumber, pageSize);
        }

        public Role GetRecordById(int id)
        {
            return context.Roles.Where(c => c.RoleId.Equals(id)).SingleOrDefault();
        }

        public IQueryable<Role> GetRecordsByFilter(Expression<Func<Role, bool>> filter)
        {
            return context.Roles.Where(filter);
        }

        public void InsertRecord(Role entity)
        {
            context.Roles.Add(entity);
        }

        public void UpdateRecord(Role entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IRoleRepository : IRepository<Role>
    {
    }
}