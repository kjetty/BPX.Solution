using BPX.DAL.UOW;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class MenuService : IMenuService
    {
        public IUnitOfWork _uow;

        public MenuService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IPagedList<Menu> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString)
        {
            throw new NotImplementedException();
        }

        public Menu GetRecordByID(int id)
        {
            return _uow.MenuRepository.GetRecordByID(id);
        }

        public IQueryable<Menu> GetRecordsByFilter(Expression<Func<Menu, bool>> filter)
        {
            return _uow.MenuRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(Menu entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.MenuRepository.InsertRecord(entity);
        }

        public void UpdateRecord(Menu entity)
        {
            //...
            //business rules validation, if any
            //...

            _uow.MenuRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            _uow.SaveDBChanges();
        }
    }

    public interface IMenuService : IGenericService<Menu>
    {
    }
}