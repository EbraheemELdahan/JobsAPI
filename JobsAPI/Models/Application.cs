using System.Text.Json.Serialization;

namespace JobsAPI.Models;

public class Application
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int JobId { get; set; }
    public int CompanyId { get; set; }
    public string? Status { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? CurrentJob { get; set; }
    public string? LinkedinUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? Logo { get; set; }
    public string? Cv { get; set; }
    public DateTime? AppliedAt { get; set; }
}