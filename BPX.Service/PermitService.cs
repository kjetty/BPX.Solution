using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
	public class PermitService : IPermitService
    {
        public PermitRepository permitRepository;

        public PermitService(IPermitRepository permitRepository)
        {
            this.permitRepository = (PermitRepository)permitRepository;
        }

        public IPagedList<Permit> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            return permitRepository.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString);
        }

        public Permit GetRecordById(int id)
        {
            return permitRepository.GetRecordById(id);
        }

        public IQueryable<Permit> GetRecordsByFilter(Expression<Func<Permit, bool>> filter)
        {
            return permitRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(Permit entity)
        {
            //...
            //business rules validation, if any
            //...

            permitRepository.InsertRecord(entity);
        }

        public void UpdateRecord(Permit entity)
        {
            //...
            //business rules validation, if any
            //...

            permitRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            permitRepository.SaveDBChanges();
        }
    }

    public interface IPermitService : IGenericService<Permit>
    {
    }
}