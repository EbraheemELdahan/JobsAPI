using System.Text.Json.Serialization;

namespace JobsAPI.Models;

public class User
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    //[JsonIgnore] // stored as hashed in json; hide from some outputs if you like
    public string? Password { get; set; }
    public string? Role { get; set; }
    public string? Phone { get; set; }
    public int? CompanyId { get; set; }

    public Company? Company { get; set; }


}