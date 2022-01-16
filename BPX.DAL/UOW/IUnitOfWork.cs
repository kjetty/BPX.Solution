using BPX.DAL.Repository;
using System;

namespace BPX.DAL.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        ILoginRepository LoginRepository { get; }
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        IUserRoleRepository UserRoleRepository { get; }
        IPermitRepository PermitRepository { get; }
        IRolePermitRepository RolePermitRepository { get; }
        IMemoryCacheKeyRepository MemoryCacheKeyRepository { get;}
        IMenuRepository MenuRepository { get; }
        IMenuRoleRepository MenuRoleRepository { get; }

        void SaveDBChanges();
    }
}