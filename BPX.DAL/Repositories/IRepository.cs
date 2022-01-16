using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repository
{
    public interface IRepository<T> where T : class
    {
        T GetRecordByID(int id);

        IPagedList<T> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString);

        IQueryable<T> GetRecordsByFilter(Expression<Func<T, bool>> filter);

        void InsertRecord(T entity);

        void UpdateRecord(T entity);
    }
}