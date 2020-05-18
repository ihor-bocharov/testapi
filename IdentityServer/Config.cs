// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Config
    {
        private const string ClientSecret = "client_secret";

        private const string LocalClientId = "LocalClientId";
        private const string ClientName = "TestClientName";

        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {
                new ApiResource("test-api", "My API")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // machine to machine client
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // scopes that client has access to
                    AllowedScopes = { "test-api" },
                    AllowAccessTokensViaBrowser = true,
                },

                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,
                
                    // where to redirect to after login
                    RedirectUris = { "http://localhost:5002/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "test-api"
                    },

                    AllowOfflineAccess = true
                },

                // Swagger client
                new Client
                {
                    ClientId = LocalClientId,
                    ClientName = ClientName,

                    AllowedGrantTypes = GrantTypes.Code,

                    AccessTokenType = AccessTokenType.Jwt,
                    IncludeJwtId = true,

                    ClientSecrets =
                    {
                        new Secret(ClientSecret.Sha256())
                    },
                    // !important for Swagger setup
                    RedirectUris = { "http://localhost:5010/swagger/oauth2-redirect.html" },
                    AllowedCorsOrigins = {"http://localhost:5010" },
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "test-api"
                    },
                    AlwaysSendClientClaims = true,
                    RequireConsent = false,
                    RequirePkce = true
                }
            };
    }
}