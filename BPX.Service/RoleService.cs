using BPX.DAL.Repositories;
using BPX.Domain.DbModels;
using System;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.Service
{
    public class RoleService : IRoleService
    {
        public RoleRepository roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            this.roleRepository = (RoleRepository)roleRepository;
        }

        public IPagedList<Role> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            return roleRepository.GetPaginatedRecords(pageNumber, pageSize, statusFlag, sortByColumn, sortOrder, searchForString, filterJson);
        }

        public Role GetRecordById(int id)
        {
            return roleRepository.GetRecordById(id);
        }

        public IQueryable<Role> GetRecordsByFilter(Expression<Func<Role, bool>> filter)
        {
            return roleRepository.GetRecordsByFilter(filter);
        }

        public void InsertRecord(Role entity)
        {
            //...
            //business rules validation, if any
            //...

            roleRepository.InsertRecord(entity);
        }

        public void UpdateRecord(Role entity)
        {
            //...
            //business rules validation, if any
            //...

            roleRepository.UpdateRecord(entity);
        }

        public void SaveDBChanges()
        {
            roleRepository.SaveDBChanges();
        }
    }

    public interface IRoleService : IGenericService<Role>
    {
    }
}