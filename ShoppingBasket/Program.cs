using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using ShoppingBasket.Repositories;
using ShoppingBasket.DbContexts;
using ShoppingBasket.Models;
using ShoppingBasket.Service; // Add this for IShoppingBasketService and ShoppingBasketService
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using ShoppingBasket.Profiles;

namespace ShoppingBasket
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
                        services.AddDbContext<ShoppingBasketDbContext>(options =>
                            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                        // Register repositories
                        services.AddScoped<IShoppingBasketRepository, ShoppingBasketRepository>();
                        services.AddScoped<IBasketLinesRepository, BasketLinesRepository>();

                        // Register ShoppingBasketService
                        services.AddScoped<IShoppingBasketService, ShoppingBasketService>();

                        // Add AutoMapper
                        services.AddAutoMapper(typeof(MappingProfile));

                        // Add controllers for API endpoints
                        services.AddControllers();

                        // Register HttpClient for making requests to ProductCatalog API
                        services.AddHttpClient();

                        // Add Swagger generation services
                        services.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShoppingBasket API", Version = "v1" });
                        });
                    });

                    webBuilder.Configure((context, app) =>
                    {
                        var env = context.HostingEnvironment;

                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }

                        // Enable Swagger middleware
                        app.UseSwagger();

                        // Enable Swagger UI
                        app.UseSwaggerUI(c =>
                        {
                            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShoppingBasket API V1");
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
