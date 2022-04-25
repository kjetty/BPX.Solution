using BPX.DAL.Context;
using BPX.Domain.DbModels;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repositories
{
	public class PortalRepository : BaseRepository, IPortalRepository
    {
        public PortalRepository(EFContext efContext, DPContext dpContext) : base(efContext, dpContext)
        {
        }

        public IPagedList<Portal> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            throw new NotImplementedException();
        }

        public Portal GetRecordById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Portal> GetRecordsByFilter(Expression<Func<Portal, bool>> filter)
        {
            return efContext.Portals.Where(filter);
        }

        public void InsertRecord(Portal entity)
        {
            efContext.Portals.Add(entity);
        }

        public void UpdateRecord(Portal entity)
        {
            efContext.Entry(entity).State = EntityState.Modified;
        }

        // dapper

        public int UpdateRecordDapper(Portal entity)
        {
            string dynQuery = "update Portals set  PToken = @PToken, LastAccessTime = @LastAccessTime where PortalUUId = @PortalUUId";
           
            DynamicParameters dynParams = new();
            dynParams.Add("PToken", entity.PToken);
            dynParams.Add("LastAccessTime", entity.LastAccessTime);
            dynParams.Add("PortalUUId", entity.PortalUUId);

            using IDbConnection connection = dpContext.CreateConnection();
            int affectedRows = connection.Execute(dynQuery, dynParams);

            //new ErrorRepository(efContext, dpContext).InsertRecordDapper(new Error { ErrorData = $"{ex.Message} {ex.StackTrace}" });

            return affectedRows;    
        }

        public Portal GetPortalByToken(string pToken)
        {
            string dynQuery = "select * from Portals where PToken = @PToken";
            
            DynamicParameters dynParams = new();
            dynParams.Add("PToken", pToken);

            using IDbConnection connection = dpContext.CreateConnection();
            Portal portal = connection.QuerySingleOrDefault<Portal>(dynQuery, dynParams);

            return portal;
        }
    }

    public interface IPortalRepository : IRepository<Portal>
    {
        int UpdateRecordDapper(Portal entity);
        Portal GetPortalByToken(string pToken);
    }
}