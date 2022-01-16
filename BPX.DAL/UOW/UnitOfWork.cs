using BPX.DAL.Context;
using BPX.DAL.Repository;

namespace BPX.DAL.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        //context
        private BPXDbContext _context;

        //repositories
        private ILoginRepository _loginRepository;
        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;
        private IUserRoleRepository _userRoleRepository;
        private IPermitRepository _permitRepository;
        private IRolePermitRepository _rolePermitRepository;
        private IMemoryCacheKeyRepository _memoryCacheKeyRepository;
        private IMenuRepository _menuRepository;
        private IMenuRoleRepository _menuRoleRepository;

        public UnitOfWork(BPXDbContext context)
        {
            _context = context;
        }

        public ILoginRepository LoginRepository { get { if (_loginRepository == null) _loginRepository = new LoginRepository(_context); return _loginRepository; } }
        public IUserRepository UserRepository { get { if (_userRepository == null) _userRepository = new UserRepository(_context); return _userRepository; } }
        public IRoleRepository RoleRepository { get { if (_roleRepository == null) _roleRepository = new RoleRepository(_context); return _roleRepository; } }
        public IUserRoleRepository UserRoleRepository { get { if (_userRoleRepository == null) _userRoleRepository = new UserRoleRepository(_context); return _userRoleRepository; } }
        public IPermitRepository PermitRepository { get { if (_permitRepository == null) _permitRepository = new PermitRepository(_context); return _permitRepository; } }
        public IRolePermitRepository RolePermitRepository { get { if (_rolePermitRepository == null) _rolePermitRepository = new RolePermitRepository(_context); return _rolePermitRepository; } }
        public IMemoryCacheKeyRepository MemoryCacheKeyRepository { get { if (_memoryCacheKeyRepository == null) _memoryCacheKeyRepository = new MemoryCacheKeyRepository(_context); return _memoryCacheKeyRepository; } }
        public IMenuRepository MenuRepository { get { if (_menuRepository == null) _menuRepository = new MenuRepository(_context); return _menuRepository; } }
        public IMenuRoleRepository MenuRoleRepository { get { if (_menuRoleRepository == null) _menuRoleRepository = new MenuRoleRepository(_context); return _menuRoleRepository; } }

        public void SaveDBChanges()
        {
            _context.SaveChanges();
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UnitOfWork() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
    }
}