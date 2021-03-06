using BPX.Domain.DbModels;
using Microsoft.EntityFrameworkCore;

namespace BPX.DAL.Context
{
    public partial class EFContext : DbContext
    {
        public EFContext(DbContextOptions<EFContext> options) : base(options)
        {
            //db.Configuration.ProxyCreationEnabled = false;
            //this.Configuration.ProxyCreationEnabled = false;            
        }

        public virtual DbSet<Portal> Portals { get; set; }
        public virtual DbSet<Login> Logins { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Permit> Permits { get; set; }
        public virtual DbSet<RolePermit> RolePermits { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<MenuPermit> MenuPermits { get; set; }
        public virtual DbSet<CacheKey> CacheKeys { get; set; }
        public virtual DbSet<Error> Errors { get; set; }
    }
}