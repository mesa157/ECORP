using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UserService.Data;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Debugging configuration
var key = builder.Configuration["Jwt:Key"] ?? "DefaultTestSecretKey12345678901234567890";
Console.WriteLine($"Loaded JWT Key: {key}");
Console.WriteLine($"Loaded JWT Issuer: {builder.Configuration["Jwt:Issuer"]}");
Console.WriteLine($"Loaded JWT Audience: {builder.Configuration["Jwt:Audience"]}");

// Add services
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        if (string.IsNullOrEmpty(key))
        {
            Console.WriteLine("JWT Key is missing or not configured correctly.");
            throw new ArgumentNullException("Jwt:Key", "JWT secret key is not configured.");
        }

        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];
        if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            Console.WriteLine("JWT Issuer or Audience is missing or not configured correctly.");
            throw new ArgumentNullException("Jwt:Issuer/Audience", "JWT Issuer or Audience is not configured.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
