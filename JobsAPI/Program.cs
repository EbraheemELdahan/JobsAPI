
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using JobsAPI.Repositories;
using JobsAPI.Services;
using JobsAPI.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Path to the JSON file from your workspace
var jsonPath = Path.Combine(builder.Environment.ContentRootPath, "json_apis.json");

// Register JsonFileStore as singleton so all repos share the same store
services.AddSingleton(new JsonFileStore(jsonPath));

// Register repositories (type-based registration already used in workspace)
services.AddSingleton(typeof(IRepository<User>), typeof(UserRepository));
services.AddSingleton(typeof(IRepository<Company>), typeof(CompanyRepository));
services.AddSingleton(typeof(IRepository<Job>), typeof(JobRepository));
services.AddSingleton(typeof(IRepository<Application>), typeof(ApplicationRepository));

// Register services
services.AddScoped<IUserService, UserService>();
services.AddScoped<ICompanyService, CompanyService>();
services.AddScoped<IJobService, JobService>();
services.AddScoped<IApplicationService, ApplicationService>();

// Register auth service
services.AddScoped<IAuthService, AuthService>();

// Read JWT config
var jwtKeyConfig = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
byte[] keyBytes;
try
{
    // try decode as base64 first (preferred)
    keyBytes = Convert.FromBase64String(jwtKeyConfig);
}
catch
{
    // fallback to interpreting as UTF8 text
    keyBytes = Encoding.UTF8.GetBytes(jwtKeyConfig);
}

// enforce minimum size: 256 bits / 32 bytes
if (keyBytes.Length < 32)
    throw new InvalidOperationException("Jwt:Key must be at least 256 bits (32 bytes). Provide a 32+ character secret or a base64-encoded 32+ byte key.");

// JWT configuration (ensure you set these in appsettings.json or environment)
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();