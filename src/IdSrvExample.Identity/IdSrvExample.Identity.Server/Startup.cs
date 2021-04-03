using IdSrvExample.Identity.Server.Configuration;
using IdSrvExample.Identity.Server.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdSrvExample.Identity.Server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = typeof(Startup).Assembly.GetName().Name;
            var connectionString = _configuration.GetConnectionString("Default");

            services.AddDbContext<AppIdentityDbContext>(options =>
            {
                //options.UseSqlServer(connectionString);
                options.UseInMemoryDatabase("IdentityDB");
            });

            services.AddIdentity<IdentityUser, IdentityRole>(config =>
                {
                    config.Password.RequiredLength = 4;
                    config.Password.RequireDigit = false;
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie";
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/Logout";
                config.AccessDeniedPath = "/Auth/AccessDenied";
            });


            services.AddIdentityServer()
                .AddAspNetIdentity<IdentityUser>()
                // this adds the operational data from DB (codes, tokens, consents)
                //.AddOperationalStore(options =>
                //{
                //    options.ConfigureDbContext = builder =>
                //        builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(assembly));
                //    // this enables automatic token cleanup. this is optional.
                //    options.EnableTokenCleanup = true;
                //    options.TokenCleanupInterval = 30; // interval in seconds
                //})
                //.AddConfigurationStore(options =>
                //{
                //    options.ConfigureDbContext = builder =>
                //        builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(assembly));
                //})
                .AddInMemoryIdentityResources(InMemory.GetIdentityResources())
                .AddInMemoryApiResources(InMemory.GetApiResources())
                .AddInMemoryClients(InMemory.GetClients())
                .AddInMemoryApiScopes(InMemory.GetApiScopes())
                .AddDeveloperSigningCredential(); 

            services.AddAuthorization();
            services.AddControllersWithViews();

            //services.AddCors(config =>
            //{
            //    config.AddPolicy("AllowAll", policy =>
            //    {
            //        policy.AllowAnyOrigin();
            //        policy.AllowAnyHeader();
            //        policy.AllowAnyMethod();
            //    });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseCors("AllowAll");

            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
