using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UnifiedFrontend.Services.UserserviceApis;
using UnifiedFrontend.Models.UserModel;
using Stripe;
using UnifiedFrontend.Services.ProductCatagory;
using UnifiedFrontend.Services.CartService;
using UnifiedFrontend.Services.PaymentServices;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configuration for backend services
builder.Services.AddSingleton(new BackendServiceOptions
{
    UserServiceUrl = builder.Configuration["BackendServices:UserService"]
});

builder.Services.AddHttpClient<PaymentService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7091/api/payments");
});

builder.Services.AddScoped<IUserServiceApi, UserServiceApi>();

// Add ProductCatalog services
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<ProductCatalogApi>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BackendServices1:ProductCatalog"]); // Replace with actual ProductCatalog URL
});

// Add CartService integration
builder.Services.AddHttpClient<CartServiceApi>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["BackendServices2:CartService"]); // Replace with actual CartService URL
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "userService",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Add routing for ProductCatalog
//app.MapControllerRoute(
   // name: "productCatalog",
   // pattern: "{controller=Products}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "categories",
    pattern: "{controller=Categories}/{action=Index}/{id?}");

// Add routing for CartService
app.MapControllerRoute(
    name: "cart",
    pattern: "{controller=Cart}/{action=Index}/{userId?}");

app.Run();
