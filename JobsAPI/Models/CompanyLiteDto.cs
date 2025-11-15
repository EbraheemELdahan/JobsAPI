using System.Text.Json.Serialization;

namespace JobsAPI.Models;

public class CompanyLiteDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Logo { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
    public string? Phone { get; set; }
    public int? CompanyId { get; set; }
    public CompanyLiteDto? Company { get; set; }
}