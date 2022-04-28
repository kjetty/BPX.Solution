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
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository(EFContext efContext, DPContext dpContext) : base(efContext, dpContext)
        {
        }

        public IPagedList<Role> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
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
            sortByColumn = sortByColumn.Length.Equals(0) ? "RoleId" : sortByColumn;
            sortOrder = sortOrder.Length.Equals(0) ? SortOrder.Ascending.ToUpper() : sortOrder;
            searchForString = searchForString.Length.Equals(0) ? string.Empty : searchForString;
            filterJson = filterJson.Length.Equals(0) ? string.Empty : filterJson;

            // get model : IQueryable : apply statusFlag
            IQueryable<Role> model = efContext.Roles.Where(c => c.StatusFlag.ToUpper().Equals(statusFlag.ToUpper()));

            // generic search
            if (searchForString.Length > 0)
            {
                model = model.Where(c => c.RoleName.ToUpper().StartsWith(searchForString.ToUpper())
                                || c.RoleDescription.ToUpper().StartsWith(searchForString.ToUpper()));
            }

            // advanced search using filters
            RoleFM roleFM = JsonConvert.DeserializeObject<RoleFM>(filterJson);

            if (roleFM != null)
            {
                if (roleFM.RoleName != null)
                    model = model.Where(c => c.RoleName.ToUpper().StartsWith(roleFM.RoleName.Trim().ToUpper()));

                if (roleFM.RoleDescription != null)
                    model = model.Where(c => c.RoleDescription.ToUpper().StartsWith(roleFM.RoleDescription.Trim().ToUpper()));
            }

            // apply sort by column, sort order
            model = sortByColumn.ToUpper() switch
            {
                "ROLENAME" => (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.RoleName) : model.OrderBy(c => c.RoleName),
                "ROLEDESCRIPTION" => (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.RoleDescription) : model.OrderBy(c => c.RoleDescription),
                _ => (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.RoleId) : model.OrderBy(c => c.RoleId),
            };

            // return ToPagedList()
            return model.ToPagedList(pageNumber, pageSize);
        }

        public Role GetRecordById(int id)
        {
            return efContext.Roles.Where(c => c.RoleId.Equals(id)).SingleOrDefault();
        }

        public IQueryable<Role> GetRecordsByFilter(Expression<Func<Role, bool>> filter)
        {
            return efContext.Roles.Where(filter);
        }

        public void InsertRecord(Role entity)
        {
            efContext.Roles.Add(entity);
        }

        public void UpdateRecord(Role entity)
        {
            efContext.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IRoleRepository : IRepository<Role>
    {
    }
}