using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class SessonService : ISessonService
    {
        public SessonRepository sessonRepository;

        public SessonService(ISessonRepository sessonRepository)
        {
            this.sessonRepository = (SessonRepository)sessonRepository;
        }

        public IPagedList<Sesson> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            throw new NotImplementedException();
        }

        public Sesson GetRecordById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Sesson> GetRecordsByFilter(Expression<Func<Sesson, bool>> filter)
        {
            return sessonRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(Sesson entity)
        {
            //...
            //business rules validation, if any
            //...

            sessonRepository.InsertRecord(entity);
        }

        public void UpdateRecord(Sesson entity)
        {
            //...
            //business rules validation, if any
            //...

            sessonRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            sessonRepository.SaveDBChanges();
        }

        // dapper

        public int UpdateRecordDapper(Sesson entity)
        {
            return sessonRepository.UpdateRecordDapper(entity);
        }

        public Sesson GetSessonByToken(string sToken)
        {
            return sessonRepository.GetSessonByToken(sToken);
        }
    }

    public interface ISessonService : IGenericService<Sesson>
    {
        int UpdateRecordDapper(Sesson entity);
        Sesson GetSessonByToken(string sToken);
    }
}