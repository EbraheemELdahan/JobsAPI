using System.Text.Json.Serialization;

namespace JobsAPI.Models;

public class Job
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string? Title { get; set; }
    public string? Company { get; set; }
    public string? Location { get; set; }
    public string? EmploymentType { get; set; }
    public string? WorkLocation { get; set; }
    public string? Category { get; set; }
    public string? Level { get; set; }
    public string? Salary { get; set; }
    public string? Description { get; set; }
    public int Applied { get; set; }
    public int Capacity { get; set; }
    public List<string>? Tags { get; set; }
    public string? Logo { get; set; }
}