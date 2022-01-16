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
            pageSize = pageSize <= 0 ? 1 : pageSize;
            statusFlag = statusFlag.Length == 0 ? RecordStatus.Active : statusFlag;
            sortByColumn = sortByColumn.Length == 0 ? "RoleId" : sortByColumn;
            sortOrder = sortOrder.Length == 0 ? SortOrder.Ascending : sortOrder;
            searchForString = searchForString.Length == 0 ? string.Empty : searchForString;

            // get model : IQueryable : apply statusFlag
            var model = _context.Roles.Where(c => c.StatusFlag == statusFlag);

            // apply search
            if (searchForString.Length > 0)
            {
                model = model.Where(c => c.RoleName.ToUpper().Contains(searchForString.Trim().ToUpper())
                                || c.RoleDescription.ToUpper().Contains(searchForString.Trim().ToUpper()));
            }

            // apply sort by column, sort order
            switch (sortByColumn)
            {
                case "FirstName":
                    model = (sortOrder == SortOrder.Descending) ? model.OrderByDescending(c => c.RoleName) : model.OrderBy(c => c.RoleName);
                    break;

                case "LastName":
                    model = (sortOrder == SortOrder.Descending) ? model.OrderByDescending(c => c.RoleDescription) : model.OrderBy(c => c.RoleDescription);
                    break;

                default:
                    model = (sortOrder == SortOrder.Descending) ? model.OrderByDescending(c => c.RoleId) : model.OrderBy(c => c.RoleId);
                    break;
            }

            // return ToPagedList()
            return model.ToPagedList(pageNumber, pageSize);
        }

        public Role GetRecordByID(int id)
        {
            return _context.Roles.Where(c => c.RoleId == id).SingleOrDefault();
        }

        public IQueryable<Role> GetRecordsByFilter(Expression<Func<Role, bool>> filter)
        {
            return _context.Roles.Where(filter);
        }

        public void InsertRecord(Role entity)
        {
            _context.Roles.Add(entity);
        }

        public void UpdateRecord(Role entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IRoleRepository : IRepository<Role>
    {
    }
}