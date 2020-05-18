// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.Ids)
                .AddInMemoryApiResources(Config.Apis)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users);


            var path = Assembly.GetExecutingAssembly().Location;

            //https://github.com/damienbod/AspNetCoreCertificates/tree/master/src/CreateIdentityServer4Certificates
            string certFile = Path.Combine(Path.GetDirectoryName(path), "rsaCert.pfx");
            string certPass = "UP912eiDfXF%fLXRk@4cD$6MG";

            // https://stackoverflow.com/questions/5036590/how-to-retrieve-certificates-from-a-pfx-file-with-c
            // Create a collection object and populate it using the PFX file
            X509Certificate2Collection certificates = new X509Certificate2Collection();
            certificates.Import(certFile, certPass, X509KeyStorageFlags.PersistKeySet);

            //builder.AddDeveloperSigningCredential();
            builder.AddSigningCredential(certificates[0]);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                );

            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}