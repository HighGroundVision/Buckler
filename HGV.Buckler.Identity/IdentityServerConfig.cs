using IdentityServer4.Models;
using System.Collections.Generic;

namespace HGV.Buckler.Identity
{
    public static class IdentityServerConfig
    {
         public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api", "API Access", new[] {"email", "discord", "steam" })
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api" }
                },
                // Postman
                new Client
                {
                    ClientId = "postman",
                    ClientSecrets = { new Secret("f4dfa43a-063b-4060-8e3e-1b860e403bb7".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://oauth.pstmn.io/v1/callback" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "api" }
                },
            };
    }
}
