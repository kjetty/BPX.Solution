using BPX.DAL.Context;
using BPX.DAL.Repositories;
using BPX.Service;
using BPX.Website.MiddleWare;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace BPX.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // inject database connection - EF - entity framework
            services.AddDbContext<EFContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("connStrDbBPX"));
                //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                //options.EnableSensitiveDataLogging();
            });

            // inject database connection - Dapper - micro orm
            services.AddSingleton<DPContext>();

            // inject service layer objects
            // SCOPED: By using this lifetime, the service will be created only once in the client request scope
            // this is particularly used in ASP.NET Core 5 where the object instance is created once per HTTP request
            // services such as Entity Framework Core's DbContext are registered with scoped lifetime

            // inject repositories (generic - basic building block)
            services.AddScoped<ISessonRepository, SessonRepository>();
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IPermitRepository, PermitRepository>();
            services.AddScoped<IRolePermitRepository, RolePermitRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IMenuPermitRepository, MenuPermitRepository>();
            services.AddScoped<ICacheKeyRepository, CacheKeyRepository>();
            services.AddScoped<IErrorRepository, ErrorRepository>();
            // inject repositories (project specific)
            // ...

            // inject services (generic - basic building block)
            services.AddScoped<ISessonService, SessonService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IPermitService, PermitService>();
            services.AddScoped<IRolePermitService, RolePermitService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IMenuPermitService, MenuPermitService>();
            services.AddScoped<ICacheKeyService, CacheKeyService>();
            services.AddScoped<IErrorService, ErrorService>();
            services.AddScoped<ICoreService, CoreService>();
            // inject services (project specific)
            // ...

            // rename anti-forgery cookie name
            services
            .AddAntiforgery(options => options.Cookie.Name = "bpx.antiforgery.cookie");

            // authentication and cookie options
            services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "bpx.website.cookie";
                //options.Cookie.Domain = "KetanJetty.com";
                options.ExpireTimeSpan = new TimeSpan(0, Convert.ToInt32(Configuration.GetSection("AppSettings").GetSection("SessionCookieTimeout").Value), 0);
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.AccessDeniedPath = "/Identity/Account/Denied";
                options.LoginPath = "/Identity/Account/Login";
            });

            // password hash options
            services.Configure<PasswordHasherOptions>(options =>
            {
                options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
                options.IterationCount = 50000;
            });

            // the local in-memory version of IDistributedCache is part of Microsoft.Extensions.Caching.Memory so is already brought in by the MVC package.
            // to use it, you need to manually add services.AddDistributedMemoryCache()
            // distributed cache
            services.AddDistributedMemoryCache();

            // inject cache
            services.AddScoped<ICacheService, CacheService>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // global exception handler
                // UseExceptionHandler("/Home/Error") sets the error handler path. 
                // If an error occurred in the MVC application then it will redirect the request to /home/error, 
                // which will execute the Error action method of HomeController.
                app.UseExceptionHandler("/Home/Error");
                
                //The default HSTS value is 30 days.You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // apply routing
            app.UseRouting();
            
            // apply authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // custom middleware
            app.UseMiddleware<ResponseTimeMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}