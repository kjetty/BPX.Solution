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
    public class SessonRepository : BaseRepository, ISessonRepository
    {
        public SessonRepository(EFContext efContext, DPContext dpContext) : base(efContext, dpContext)
        {
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
            return efContext.Sessons.Where(filter);
        }

        public void InsertRecord(Sesson entity)
        {
            efContext.Sessons.Add(entity);
        }

        public void UpdateRecord(Sesson entity)
        {
            efContext.Entry(entity).State = EntityState.Modified;
        }

        // dapper

        public int UpdateRecordDapper(Sesson entity)
        {
            string dynQuery = "update Sessons set SToken = @SToken, LastAccessTime = @LastAccessTime where SessonUUId = @SessonUUId";

            DynamicParameters dynParams = new();
            dynParams.Add("SToken", entity.SToken);
            dynParams.Add("LastAccessTime", entity.LastAccessTime);
            dynParams.Add("SessonUUId", entity.SessonUUId);

            using IDbConnection connection = dpContext.CreateConnection();
            int affectedRows = connection.Execute(dynQuery, dynParams);

            //new ErrorRepository(efContext, dpContext).InsertRecordDapper(new Error { ErrorData = $"{ex.Message} {ex.StackTrace}" });

            return affectedRows;
        }

        public Sesson GetSessonByToken(string sToken)
        {
            string dynQuery = "select SessonUUId, SToken, LastAccessTime from Sessons where SToken = @SToken";

            DynamicParameters dynParams = new();
            dynParams.Add("SToken", sToken);

            using IDbConnection connection = dpContext.CreateConnection();
            Sesson sesson = connection.QuerySingleOrDefault<Sesson>(dynQuery, dynParams);

            return sesson;
        }
    }

    public interface ISessonRepository : IRepository<Sesson>
    {
        int UpdateRecordDapper(Sesson entity);
        Sesson GetSessonByToken(string sToken);
    }
}