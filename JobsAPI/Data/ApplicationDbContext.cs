using Microsoft.EntityFrameworkCore;
using JobsAPI.Models;

namespace JobsAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobsAPI.Models.Application> Applications => Set<JobsAPI.Models.Application>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");
            b.HasKey(u => u.Id);
            b.Property(u => u.Id).HasColumnName("id");
            b.Property(u => u.UserName).HasColumnName("username");
            b.Property(u => u.Email).HasColumnName("email");
            b.Property(u => u.Password).HasColumnName("password");
            b.Property(u => u.Role).HasColumnName("role");
            b.Property(u => u.Phone).HasColumnName("phone");
            b.Property(u => u.CompanyId).HasColumnName("companyid");

            // Relationship: User -> Company (many-to-one)
            b.HasOne(u => u.Company)
             .WithMany(c => c.Users)
             .HasForeignKey(u => u.CompanyId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Company>(b =>
        {
            b.ToTable("companies");
            b.HasKey(c => c.Id);
            b.Property(c => c.Id).HasColumnName("id");
            b.Property(c => c.Name).HasColumnName("name");
            b.Property(c => c.Logo).HasColumnName("logo");
            b.Property(c => c.Location).HasColumnName("location");
            b.Property(c => c.Industry).HasColumnName("industry");
            b.Property(c => c.Description).HasColumnName("description");
            b.Property(c => c.Category).HasColumnName("category");
            b.Property(c => c.Jobs).HasColumnName("jobs");
            b.Property(c => c.Website).HasColumnName("website");
            b.Property(c => c.Tags).HasColumnType("jsonb").HasColumnName("tags");
            b.Property(c => c.Stats).HasColumnType("jsonb").HasColumnName("stats");
        });

        modelBuilder.Entity<Job>(b =>
        {
            b.ToTable("jobs");
            b.HasKey(j => j.Id);
            b.Property(j => j.Id).HasColumnName("id");
            b.Property(j => j.CompanyId).HasColumnName("companyid");
            b.Property(j => j.Title).HasColumnName("title");
            b.Property(j => j.Company).HasColumnName("company");
            b.Property(j => j.Location).HasColumnName("location");
            b.Property(j => j.EmploymentType).HasColumnName("employmenttype");
            b.Property(j => j.WorkLocation).HasColumnName("worklocation");
            b.Property(j => j.Category).HasColumnName("category");
            b.Property(j => j.Level).HasColumnName("level");
            b.Property(j => j.Salary).HasColumnName("salary");
            b.Property(j => j.Description).HasColumnName("description");
            b.Property(j => j.Applied).HasColumnName("applied");
            b.Property(j => j.Capacity).HasColumnName("capacity");
            b.Property(j => j.Tags).HasColumnType("jsonb").HasColumnName("tags");
            b.Property(j => j.Logo).HasColumnName("logo");
        });

        modelBuilder.Entity<JobsAPI.Models.Application>(b =>
        {
            b.ToTable("applications");
            b.HasKey(a => a.Id);
            b.Property(a => a.Id).HasColumnName("id");
            b.Property(a => a.UserId).HasColumnName("userid");
            b.Property(a => a.JobId).HasColumnName("jobid");
            b.Property(a => a.CompanyId).HasColumnName("companyid");
            b.Property(a => a.Status).HasColumnName("status");
            b.Property(a => a.FullName).HasColumnName("fullname");
            b.Property(a => a.Email).HasColumnName("email");
            b.Property(a => a.Phone).HasColumnName("phone");
            b.Property(a => a.CurrentJob).HasColumnName("currentjob");
            b.Property(a => a.LinkedinUrl).HasColumnName("linkedinurl");
            b.Property(a => a.PortfolioUrl).HasColumnName("portfoliourl");
            b.Property(a => a.AdditionalInfo).HasColumnName("additionalinfo");
            b.Property(a => a.Logo).HasColumnName("logo");
            b.Property(a => a.Cv).HasColumnName("cv");
            b.Property(a => a.AppliedAt).HasColumnName("appliedat");
        });
    }
}