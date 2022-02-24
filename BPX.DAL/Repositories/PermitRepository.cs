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
	public class PermitRepository : BaseRepository, IPermitRepository
    {
        public PermitRepository(BPXDbContext context) : base(context)
        {
        }

        public IPagedList<Permit> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
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
            sortByColumn = sortByColumn.Length.Equals(0) ? "PermitId" : sortByColumn;
            sortOrder = sortOrder.Length.Equals(0) ? SortOrder.Ascending : sortOrder;
            searchForString = searchForString.Length.Equals(0) ? string.Empty : searchForString;

            // get model : IQueryable : apply statusFlag
            IQueryable<Permit> model = context.Permits.Where(c => c.StatusFlag.ToUpper().Equals(statusFlag.ToUpper()));

            // apply search
            if (searchForString.Length > 0)
            {
                model = model.Where(c => c.PermitArea.ToUpper().Contains(searchForString.Trim().ToUpper())
                                || c.PermitController.ToUpper().Contains(searchForString.Trim().ToUpper())
                                || c.PermitName.ToUpper().Contains(searchForString.Trim().ToUpper())
                                || c.PermitEnum.ToUpper().Contains(searchForString.Trim().ToUpper()));
            }

            // apply sort by column, sort order
            switch (sortByColumn.ToUpper())
            {
                case "PERMITAREA":
                    model = (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.PermitArea) : model.OrderBy(c => c.PermitArea);
                    break;

                case "PERMITCONTROLLER":
                    model = (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.PermitController) : model.OrderBy(c => c.PermitController);
                    break;

                case "PERMITNAME":
                    model = (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.PermitName) : model.OrderBy(c => c.PermitName);
                    break;

                case "PERMITENUM":
                    model = (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.PermitEnum) : model.OrderBy(c => c.PermitEnum);
                    break;

                default:
                    model = (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.PermitId) : model.OrderBy(c => c.PermitId);
                    break;
            }

            // return ToPagedList()
            return model.ToPagedList(pageNumber, pageSize);
        }

        public Permit GetRecordById(int id)
        {
            return context.Permits.Where(c => c.PermitId.Equals(id)).SingleOrDefault();
        }

        public IQueryable<Permit> GetRecordsByFilter(Expression<Func<Permit, bool>> filter)
        {
            return context.Permits.Where(filter);
        }

        public void InsertRecord(Permit entity)
        {
            context.Permits.Add(entity);
        }

        public void UpdateRecord(Permit entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IPermitRepository : IRepository<Permit>
    {
    }
}