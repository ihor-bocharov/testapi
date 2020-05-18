using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TestApi.Middleware;

namespace TestApi
{
    public class Startup
    {
        private const string LocalClientId = "LocalClientId";
        private const string ClientName = "TestClientName";

        private const string ClientSecret = "client_secret";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var authority = "http://localhost:5000";
            var key = "secret";

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                // .well-known/oauth-authorization-server or .well-known/openid-configuration
                "http://localhost:5000/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever() { RequireHttps = false });

            var discoveryDocument = 
                configurationManager
                    .GetConfigurationAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

            var signingKeys = discoveryDocument.SigningKeys;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

                              // Validate Token
                              .AddJwtBearer(options =>
                              {

                                  options.TokenValidationParameters = new TokenValidationParameters
                                  {
                                      // Clock skew compensates for server time drift.
                                      // We recommend 5 minutes or less:
                                      ClockSkew = TimeSpan.FromMinutes(5),

                                      // Specify the key used to sign the token:
                                      IssuerSigningKey = signingKeys.First(),

                                      RequireSignedTokens = true,

                                      // Ensure the token hasn't expired:
                                      RequireExpirationTime = true,

                                      ValidateLifetime = true,

                                      // Ensure the token audience matches our audience value (default true):
                                      ValidateAudience = true,

                                      ValidAudience = "test-api",

                                      // Ensure the token was issued by a trusted authorization server (default true):
                                      ValidateIssuer = true,

                                      ValidIssuer = authority
                                  };
                              });

            //.AddJwtBearer("Bearer", options =>
            //{
            //    options.Authority = issuer;
            //    options.RequireHttpsMetadata = false;
            //    options.Audience = "test-api";
            //    options.SaveToken = true;
            //});

            services.AddMvc();
            //services.AddMvcCore().AddApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "My API",
                        Version = "v1",
                        Description = "A sample API to demo Swashbuckle",
                        License = new OpenApiLicense
                        {
                            Name = "Apache 2.0",
                            Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html")
                        }
                    });

                c.OperationFilter<SecurityRequirementsOperationFilter>();

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        /*
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("http://localhost:5000/connect/authorize", UriKind.RelativeOrAbsolute),
                            TokenUrl = new Uri("http://localhost:5000/connect/token", UriKind.RelativeOrAbsolute),
                            Scopes = new Dictionary<string, string>
                            {
                                {"test-api", "Access read operations"}
                            }
                        },
                        */
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("http://localhost:5000/connect/authorize", UriKind.RelativeOrAbsolute),
                            TokenUrl = new Uri("http://localhost:5000/connect/token", UriKind.RelativeOrAbsolute),
                            Scopes = new Dictionary<string, string>
                            {
                                {"test-api", "Access read operations"}
                            }
                        }
                    },

                    Description = "OpenId Security Scheme"
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                options.OAuthClientSecret(ClientSecret);
                options.OAuthClientId(LocalClientId);
                options.OAuthAppName("Demo API - Swagger");
                options.OAuthScopeSeparator(" ");
                options.OAuthUsePkce();
            });
        }
    }
}
