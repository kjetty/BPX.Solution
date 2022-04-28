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
    public class PermitRepository : BaseRepository, IPermitRepository
    {
        public PermitRepository(EFContext efContext, DPContext dpContext) : base(efContext, dpContext)
        {
        }

        public IPagedList<Permit> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
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
            sortByColumn = sortByColumn.Length.Equals(0) ? "PermitId" : sortByColumn;
            sortOrder = sortOrder.Length.Equals(0) ? SortOrder.Ascending.ToUpper() : sortOrder;
            searchForString = searchForString.Length.Equals(0) ? string.Empty : searchForString;
            filterJson = filterJson.Length.Equals(0) ? string.Empty : filterJson;

            // get model : IQueryable : apply statusFlag
            IQueryable<Permit> model = efContext.Permits.Where(c => c.StatusFlag.ToUpper().Equals(statusFlag.ToUpper()));

            // generic search
            if (searchForString.Length > 0)
            {
                model = model.Where(c => c.PermitArea.ToUpper().StartsWith(searchForString.Trim().ToUpper())
                                || c.PermitController.ToUpper().StartsWith(searchForString.Trim().ToUpper())
                                || c.PermitName.ToUpper().StartsWith(searchForString.Trim().ToUpper())
                                || c.PermitEnum.ToUpper().StartsWith(searchForString.Trim().ToUpper()));
            }

            // advanced search using filters
            PermitFM permitFM = JsonConvert.DeserializeObject<PermitFM>(filterJson);

            if (permitFM != null)
            {
                if (permitFM.PermitArea != null)
                    model = model.Where(c => c.PermitArea.ToUpper().StartsWith(permitFM.PermitArea.Trim().ToUpper()));

                if (permitFM.PermitController != null)
                    model = model.Where(c => c.PermitController.ToUpper().StartsWith(permitFM.PermitController.Trim().ToUpper()));

                if (permitFM.PermitName != null)
                    model = model.Where(c => c.PermitName.ToUpper().StartsWith(permitFM.PermitName.Trim().ToUpper()));

                if (permitFM.PermitEnum != null)
                    model = model.Where(c => c.PermitEnum.ToUpper().StartsWith(permitFM.PermitEnum.Trim().ToUpper()));
            }

            // apply sort by column, sort order
            model = sortByColumn.ToUpper() switch
            {
                "PERMITAREA" => (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.PermitArea) : model.OrderBy(c => c.PermitArea),
                "PERMITCONTROLLER" => (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.PermitController) : model.OrderBy(c => c.PermitController),
                "PERMITNAME" => (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.PermitName) : model.OrderBy(c => c.PermitName),
                "PERMITENUM" => (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.PermitEnum) : model.OrderBy(c => c.PermitEnum),
                _ => (sortOrder.ToUpper().Equals(SortOrder.Descending.ToUpper())) ? model.OrderByDescending(c => c.PermitId) : model.OrderBy(c => c.PermitId),
            };

            // return ToPagedList()
            return model.ToPagedList(pageNumber, pageSize);
        }

        public Permit GetRecordById(int id)
        {
            return efContext.Permits.Where(c => c.PermitId.Equals(id)).SingleOrDefault();
        }

        public IQueryable<Permit> GetRecordsByFilter(Expression<Func<Permit, bool>> filter)
        {
            return efContext.Permits.Where(filter);
        }

        public void InsertRecord(Permit entity)
        {
            efContext.Permits.Add(entity);
        }

        public void UpdateRecord(Permit entity)
        {
            efContext.Entry(entity).State = EntityState.Modified;
        }
    }

    public interface IPermitRepository : IRepository<Permit>
    {
    }
}