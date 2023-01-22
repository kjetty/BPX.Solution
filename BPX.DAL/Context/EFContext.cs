using BPX.Domain.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;

namespace BPX.DAL.Context
{
    public partial class EFContext : DbContext
    {
        public EFContext(DbContextOptions<EFContext> options) : base(options)
        {
            //this.ChangeTracker.LazyLoadingEnabled = false;

            //Do not enable lazy loading
            //Lazy loading is a feature that has caused countless production issues, and the EF Core team has rightfully
            //made it inconvenient to enable this feature.

            //It’s highly advised not to enable this feature, as navigation properties become opaque mechanisms
            //that trigger unnecessary and costly roundtrips to the database.

        }

        // auto generated when scaffolding to add connection string 
        // it is advisable to remove this and use appsettings.json / Startup.cs to set up connection string and DbContext
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
                //To enable lazy loading, folks have to install the Microsoft.EntityFrameworkCore.Proxies package.

                //dotnet add package Microsoft.EntityFrameworkCore.Proxies
                //Then, from the OnConfiguring method of a DbContext, we can call the UseLazyLoadingProxies method.

                //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseLazyLoadingProxies().UseSqlServer(myConnectionString);


        //    optionsBuilder.UseSqlServer("Data Source=xxx;Initial Catalog=xxx;Integrated Security=False;User Id=xxx;Password=xxx;MultipleActiveResultSets=True");
        //}

        // If you follow the Entity framework model naming convention and your model directly reflects your database table name,
        // column names and so on, you don't need the OnMOdelCreating().

        // Entity framework will generate the bindings between the Domain class and Database tables behind the scene.

        // But, if you want customization, for example, your model field name does not match your database table column name,
        // you configure that on the OnModelCreating method using fluent API.

        // This doesn't mean you have to use the OnModelCreating method.
        // There are other options for customization. Which is DataAnotation on the Domain classes.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //throw new UnintentionalCodeFirstException();

            // to cnfigure domain classes using modelBuilder and fluentAPI

            // example
            //modelBuilder.Entity<User>(entity => {
            //    entity.Property(p => p.Password)
            //          .HasColumnName("Pwd");
            //})

            // other way of mapping is using Data Annotation in the Domain class 
            //[Column("Pwd")]
            //public string Password { get; set; }
        }

        public virtual DbSet<Sesson> Sessons { get; set; }
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