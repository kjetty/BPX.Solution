using BPX.Domain.DbModels;
using Microsoft.EntityFrameworkCore;

namespace BPX.DAL.Context
{
    public partial class BPXDbContext : DbContext
    {
        public BPXDbContext(DbContextOptions<BPXDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Login> Logins { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Permit> Permits { get; set; }
        public virtual DbSet<RolePermit> RolePermits { get; set; }
        public virtual DbSet<CacheKey> CacheKeys { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<MenuPermit> MenuPermits { get; set; }
    }
}