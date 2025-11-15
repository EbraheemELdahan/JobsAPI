using System.Text.Json.Serialization;

namespace JobsAPI.Models;

public class Company
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Logo { get; set; }
    public string? Location { get; set; }
    public string? Industry { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int Jobs { get; set; }
    public string? Website { get; set; }
    public List<string>? Tags { get; set; }
    public List<CompanyStat>? Stats { get; set; }
}

public class CompanyStat
{
    public string? Label { get; set; }
    public string? Value { get; set; }
}