using BPX.DAL.Context;
using BPX.DAL.Repository;

namespace BPX.DAL.UOW
{
	public class UnitOfWork : IUnitOfWork
	{
		//context
		private readonly BPXDbContext _context;

		//repositories
		public ILoginRepository LoginRepository { get; }
		public IUserRepository UserRepository { get; }
		public IRoleRepository RoleRepository { get; }
		public IUserRoleRepository UserRoleRepository { get; }
		public IPermitRepository PermitRepository { get; }
		public IRolePermitRepository RolePermitRepository { get; }
		public IMemoryCacheKeyRepository MemoryCacheKeyRepository { get; }
		public IMenuRepository MenuRepository { get; }
		public IMenuRoleRepository MenuRoleRepository { get; }

		public UnitOfWork(BPXDbContext context,ILoginRepository loginRepository, IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, IPermitRepository permitRepository, IRolePermitRepository rolePermitRepository, IMemoryCacheKeyRepository memoryCacheKeyRepository, IMenuRepository menuRepository, IMenuRoleRepository menuRoleRepository)
		{
			_context = context;
			this.LoginRepository = loginRepository;
			this.UserRepository = userRepository;
			this.RoleRepository = roleRepository;
			this.UserRoleRepository = userRoleRepository;
			this.PermitRepository = permitRepository;
			this.RolePermitRepository = rolePermitRepository;
			this.MemoryCacheKeyRepository = memoryCacheKeyRepository;
			this.MenuRepository = menuRepository;
			this.MenuRoleRepository = menuRoleRepository;
		}

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