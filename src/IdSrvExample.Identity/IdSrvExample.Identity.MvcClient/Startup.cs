using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace IdSrvExample.Identity.TestMvcClient
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
            services.AddAuthentication(config =>
                {
                    config.DefaultScheme = "Cookie";
                    config.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookie", config =>
                {
                    config.AccessDeniedPath = "/AccessDenied";
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookie";
                    options.Authority = "https://localhost:5000";
                    options.RequireHttpsMetadata = true;
                    options.ClientId = "client_id_mvc";
                    options.ClientSecret = "client_secret_mvc";
                    options.SaveTokens = true;
                    options.ResponseType = "code";
                    options.SignedOutCallbackPath = "/Home/Index";
                    options.UsePkce = true;

                    // id token will be smaller, but user claims will be loaded via an additional call
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.ClaimActions.DeleteClaim("amr");
                    options.ClaimActions.DeleteClaim("s_hash");
                    options.ClaimActions.MapUniqueJsonKey("MVCClient.custom", "IdSrvExample.custom");
                    options.ClaimActions.MapJsonKey(JwtClaimTypes.Role, JwtClaimTypes.Role);
                    options.ClaimActions.MapJsonKey(JwtClaimTypes.Subject, JwtClaimTypes.Subject);

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("IdSrvExample.assets");
                    options.Scope.Add("IdSrvExample.scope");
                    //options.Scope.Add("IdSrvExample.roles");
                    options.Scope.Add("offline_access");

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        NameClaimType = JwtClaimTypes.Subject,
                        RoleClaimType = JwtClaimTypes.Role
                    };

                    options.Events.OnTokenValidated += context =>
                    {
                        return Task.CompletedTask;
                    };
                });

            services.AddHttpClient();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
