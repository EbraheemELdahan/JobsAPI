using System.Text.Json;
using JobsAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace JobsAPI.Data;

public static class JsonSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var jsonFile = Path.Combine(env.ContentRootPath, "json_apis.json");
        if (!File.Exists(jsonFile)) return;

        // If DB already has data, skip seeding to avoid duplicates
        if (await db.Users.AnyAsync() || await db.Companies.AnyAsync() || await db.Jobs.AnyAsync() || await db.Applications.AnyAsync())
            return;

        var doc = await JsonDocument.ParseAsync(File.OpenRead(jsonFile));
        var root = doc.RootElement;

        if (root.TryGetProperty("users", out var usersEl) && usersEl.ValueKind == JsonValueKind.Array)
        {
            var users = JsonSerializer.Deserialize<List<User>>(usersEl.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            if (users?.Count > 0)
            {
                db.Users.AddRange(users);
            }
        }

        if (root.TryGetProperty("companies", out var companiesEl) && companiesEl.ValueKind == JsonValueKind.Array)
        {
            var companies = JsonSerializer.Deserialize<List<Company>>(companiesEl.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            if (companies?.Count > 0)
            {
                db.Companies.AddRange(companies);
            }
        }

        if (root.TryGetProperty("jobs", out var jobsEl) && jobsEl.ValueKind == JsonValueKind.Array)
        {
            var jobs = JsonSerializer.Deserialize<List<Job>>(jobsEl.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            if (jobs?.Count > 0)
            {
                db.Jobs.AddRange(jobs);
            }
        }

        if (root.TryGetProperty("applications", out var appsEl) && appsEl.ValueKind == JsonValueKind.Array)
        {
            var apps = JsonSerializer.Deserialize<List<JobsAPI.Models.Application>>(appsEl.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            if (apps?.Count > 0)
            {
                db.Applications.AddRange(apps);
            }
        }

        await db.SaveChangesAsync();
    }
}