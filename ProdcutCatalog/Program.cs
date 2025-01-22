using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using ProdcutCatalog.Repositories;
using ProdcutCatalog.DbContexts;
using ProdcutCatalog.Entities;
using System;

namespace ProductCatalog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices((hostContext, services) =>
                    {
                        // Register DbContext with SQL Server (use your connection string here)
                        services.AddDbContext<ProductCatalogDbContext>(options =>
                            options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ProductCatalog;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));

                        // Register repositories
                        services.AddScoped<ICategoryRepository, CategoryRepository>();
                        services.AddScoped<IProductRepository, ProductRepository>();

                        // Add AutoMapper
                        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                        // Add controllers for API endpoints
                        services.AddControllers();

                        // Add Swagger generation services
                        services.AddSwaggerGen(); // Add Swagger services here
                    });

                    webBuilder.Configure((context, app) =>
                    {
                        var env = context.HostingEnvironment;

                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }

                        // Enable Swagger middleware
                        app.UseSwagger(); // Enabling Swagger

                        // Enable Swagger UI
                        app.UseSwaggerUI(c =>
                        {
                            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductCatalog API V1");
                            c.RoutePrefix = "swagger"; // Set route for Swagger UI
                        });

                        // Enable other middlewares
                        app.UseHttpsRedirection();
                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                });
    }
}
