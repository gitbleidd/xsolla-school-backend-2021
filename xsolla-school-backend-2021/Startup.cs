using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using XsollaSchoolBackend.Data;
using XsollaSchoolBackend.Database;

namespace XsollaSchoolBackend
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            string dbFolderPath = System.IO.Path.Combine(AppContext.BaseDirectory, "db");
            Directory.CreateDirectory(dbFolderPath);
            string dbPath = System.IO.Path.Combine(dbFolderPath, "shop.db");


            services.AddSingleton(new Database.DatabaseConfig { Name = "Data Source=" + dbPath });
            services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();
            services.AddSingleton<IItemRepository, SqliteItemRepository>();
            //services.AddSingleton<IItemRepository, InMemoryItemRepository>();
            services.AddSingleton<ICommentRepository, CommentRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Система управления товарами",
                    Version = "v1",
                    Description = "Web API, в котором реализовано управление товарами с помощью RESTful API."
                });

                c.CustomOperationIds(apiDescription =>
                {
                    return apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services
                .AddGraphQLServer()
                .AddQueryType<GraphQL.Query>()
                .AddMutationType<GraphQL.Mutation>();

            services.AddGrpc();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                // Cookie settings
                .AddCookie(options =>
                {
                    options.LoginPath = "/api/account/google-login";
                    options.Cookie.HttpOnly = true;
                })
                .AddGoogle(options =>
                {
                    options.ClientId = System.Environment.GetEnvironmentVariable("GoogleAuthId");
                    options.ClientSecret = System.Environment.GetEnvironmentVariable("GoogleAuthSecret");
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDatabaseBootstrap dbBootstrap)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "XsollaSchoolBackend v1");
                    c.DisplayOperationId();
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGraphQL()
                .WithOptions(new HotChocolate.AspNetCore.GraphQLServerOptions { Tool = { Enable = env.IsDevelopment() } });
            });

            dbBootstrap.Setup();
        }
    }
}
