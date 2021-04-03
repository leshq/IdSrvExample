using System.Collections.Generic;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace IdSrvExample.Identity.Server.Configuration
{
    public class InMemory
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResource(
                    "IdSrvExample.scope",
                    new[]
                    {
                        "IdSrvExample.custom",
                        JwtClaimTypes.Role
                    }),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("IdSrvExample.assets")
                {
                    UserClaims = new[] {JwtClaimTypes.Role},
                    Scopes = new []{"IdSrvExample.assets"},
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("IdSrvExample.assets")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client {
                    ClientId = "client_id_mvc",
                    ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    RedirectUris = { "https://localhost:5020/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5020/Home/Index" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "IdSrvExample.scope",
                        "IdSrvExample.assets"
                    },

                    // puts all the claims in the id token  
                    //AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                },
                new Client {
                    ClientId = "client_id_js",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "https://localhost:50050/home/signin" },
                    PostLogoutRedirectUris = { "https://localhost:50050/Home/Index" },
                    AllowedCorsOrigins = { "https://localhost:50050" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "IdSrvExample.scope",
                        "IdSrvExample.assets"
                    },

                    AccessTokenLifetime = 1,

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                },
                new Client {
                    ClientId = "client_id_angular",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "http://localhost:4200" },
                    PostLogoutRedirectUris = { "http://localhost:4200" },
                    AllowedCorsOrigins = { "http://localhost:4200" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "IdSrvExample.scope",
                        "IdSrvExample.assets"
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                },
                new Client
                {
                    ClientId = "postman",
                    ClientName = "Postman",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        "openid",
                        "profile",
                        "email",
                        "IdSrvExample.assets",
                        "IdSrvExample.scope"
                    },
                }
            };
        }
    }
}
