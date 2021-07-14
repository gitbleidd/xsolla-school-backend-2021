using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
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

            string dbFolderPath = Path.Combine(AppContext.BaseDirectory, "db");
            Directory.CreateDirectory(dbFolderPath);
            string dbPath = Path.Combine(dbFolderPath, "shop.db");


            services.AddSingleton(new Database.DatabaseConfig { Name = "Data Source=" + dbPath });
            services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();
            services.AddSingleton<IItemRepository, SqliteItemRepository>();
            //services.AddSingleton<IItemRepository, InMemoryItemRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "Система управления товарами", 
                    Version = "v1", 
                    Description = "Web API, в котором реализовано управление товарами с помощью RESTful API."
                });

                c.CustomOperationIds(apiDescription =>
                {
                    return apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
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

            //app.UseHttpsRedirection();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            dbBootstrap.Setup();
        }
    }
}
