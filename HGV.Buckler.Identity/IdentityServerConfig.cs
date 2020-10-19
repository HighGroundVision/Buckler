using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace HGV.Buckler.Identity
{
    public static class IdentityServerConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources(IConfiguration configuration)
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
            };
        }
            
        public static IEnumerable<ApiScope> ApiScopes(IConfiguration configuration)
        {
            return new List<ApiScope>
            {
                new ApiScope("api", "API Access"),
                new ApiScope("email", "Verified Email Address", new[] { "email" }),
                new ApiScope("discord", "Discord Id", new[] { "discord_id" }),
                new ApiScope("steam", "Steam Identity", new[] { "steam_id", "steam_persona", "steam_avatar" })
            };
        }   

        public static IEnumerable<Client> Clients(IConfiguration configuration)
        {
            return new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret(configuration["IdentityServer:Clients:m2m:Key"].Sha256()) },

                    AllowedScopes = { "api" },
                    
                },
                // JS Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    ClientSecrets = { new Secret(configuration["IdentityServer:Clients:js:Key"].Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,

                    RedirectUris =           { configuration["IdentityServer:Clients:js:Uri"] + "/callback" },
                    PostLogoutRedirectUris = { configuration["IdentityServer:Clients:js:Uri"] },
                    // AllowedCorsOrigins =     { "*" },  // { configuration["IdentityServer:Clients:js:Uri"] },
                    
                    AllowedScopes = { "openid", "api", "email", "discord", "steam" }
                },
                // Postman
                new Client
                {
                    ClientId = "postman",
                    ClientName = "Postman Client",
                    ClientSecrets = { new Secret(configuration["IdentityServer:Clients:postman:Key"].Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { configuration["IdentityServer:Clients:postman:Uri"] },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "api", "email", "discord", "steam" }
                },
            };
        }
    }
}
