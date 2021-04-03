using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdSrvExample.Identity.Server.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdSrvExample.Identity.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            SeedDatabase(host);

            host.Run();
        }

        private static void SeedDatabase(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                SeedUsers(scope);
                //SeedIdentityConfig(scope);
            }
        }

        private static void SeedUsers(IServiceScope scope)
        {
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            var bob = new IdentityUser("bob");
            userManager.CreateAsync(bob, "password").GetAwaiter().GetResult();
            userManager.AddClaimAsync(bob, new Claim("IdSrvExample.custom", "my custom claim")).GetAwaiter().GetResult();
            userManager.AddClaimAsync(bob, new Claim(JwtClaimTypes.Role, "basic")).GetAwaiter().GetResult();

            var admin = new IdentityUser("admin");
            userManager.CreateAsync(admin, "password").GetAwaiter().GetResult();
            userManager.AddClaimAsync(admin, new Claim(JwtClaimTypes.Role, "admin")).GetAwaiter().GetResult();

            Console.WriteLine($"Bob user ID: {bob.Id}");
            Console.WriteLine($"Admin user ID: {admin.Id}");
        }

        private static void SeedIdentityConfig(IServiceScope scope)
        {
            scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>()
                .Database.Migrate();

            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            context.Database.Migrate();

            if (!context.Clients.Any())
            {
                foreach (var client in InMemory.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in InMemory.GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in InMemory.GetApiResources())
                {
                    context.ApiResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var apiScope in InMemory.GetApiScopes())
                {
                    context.ApiScopes.Add(apiScope.ToEntity());
                }

                context.SaveChanges();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
