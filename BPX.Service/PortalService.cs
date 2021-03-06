using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class PortalService : IPortalService
    {
        public PortalRepository portalRepository;

        public PortalService(IPortalRepository portalRepository)
        {
            this.portalRepository = (PortalRepository)portalRepository;
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
            return portalRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(Portal entity)
        {
            //...
            //business rules validation, if any
            //...

            portalRepository.InsertRecord(entity);
        }

        public void UpdateRecord(Portal entity)
        {
            //...
            //business rules validation, if any
            //...

            portalRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            portalRepository.SaveDBChanges();
        }

        // dapper

        public int UpdateRecordDapper(Portal entity)
        {
            return portalRepository.UpdateRecordDapper(entity);
        }

        public Portal GetPortalByToken(string pToken)
        {
            return portalRepository.GetPortalByToken(pToken);
        }
    }

    public interface IPortalService : IGenericService<Portal>
    {
        int UpdateRecordDapper(Portal entity);
        Portal GetPortalByToken(string pToken);
    }
}