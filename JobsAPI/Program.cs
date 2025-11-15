using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using JobsAPI.Data;
using JobsAPI.Models;
using JobsAPI.Repositories;
using JobsAPI.Services;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Enable dynamic JSON support for System.Text.Json (required by Npgsql 8+)
NpgsqlConnection.GlobalTypeMapper.EnableDynamicJson();

// Read the raw connection string from configuration
var rawConn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? throw new InvalidOperationException("DefaultConnection not configured");

string effectiveConn;

// If the connection string looks like a URI (postgres:// or postgresql://), convert it
if (rawConn.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
    rawConn.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
{
    var uri = new Uri(rawConn);
    var userInfo = uri.UserInfo.Split(':', 2);
    var username = Uri.UnescapeDataString(userInfo.ElementAtOrDefault(0) ?? "");
    var password = Uri.UnescapeDataString(userInfo.ElementAtOrDefault(1) ?? "");
    var database = uri.AbsolutePath?.TrimStart('/') ?? "";

    var builderConn = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.IsDefaultPort ? 5432 : uri.Port,
        Username = username,
        Password = password,
        Database = database,
        // Railway and other hosted providers usually require SSL
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    };

    effectiveConn = builderConn.ConnectionString;
}
else
{
    // assume it's already a key/value connection string
    effectiveConn = rawConn;
}

// Register DbContext with the effective connection string
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(effectiveConn));

// Register repositories: generic EF repository
services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

// Register services
services.AddScoped<IUserService, UserService>();
services.AddScoped<ICompanyService, CompanyService>();
services.AddScoped<IJobService, JobService>();
services.AddScoped<IApplicationService, ApplicationService>();
services.AddScoped<IAuthService, AuthService>();

// JWT config from appsettings (ensure key is at least 32 bytes or base64 32+ bytes)
var jwtKeyConfig = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
byte[] keyBytes;
try { keyBytes = Convert.FromBase64String(jwtKeyConfig); } catch { keyBytes = Encoding.UTF8.GetBytes(jwtKeyConfig); }
if (keyBytes.Length < 32) throw new InvalidOperationException("Jwt:Key must be at least 256 bits (32 bytes).");

var issuer = builder.Configuration["Jwt:Issuer"] ?? "JobsAPI";
var audience = builder.Configuration["Jwt:Audience"] ?? "JobsAPIUsers";

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

services.AddAuthorization();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations and seed JSON data (safe to run in dev)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    await JsonSeeder.SeedAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();