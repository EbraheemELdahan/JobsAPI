using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JobsAPI.Models;
using JobsAPI.Repositories;

namespace JobsAPI.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly IRepository<User> _userRepo;

    public AuthService(IConfiguration config, IRepository<User> userRepo)
    {
        _config = config;
        _userRepo = userRepo;
    }

    public async Task<User?> ValidateCredentialsAsync(string email, string password)
    {
        var users = await _userRepo.GetAllAsync();
        var user = users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        if (user == null || string.IsNullOrEmpty(user.Password))
            return null;

        var stored = user.Password!;

        if (stored.StartsWith("$2"))
        {
            try
            {
                if (!BCrypt.Net.BCrypt.Verify(password, stored))
                    return null;

                return user;
            }
            catch
            {
                return null;
            }
        }

        if (stored == password)
        {
            var hashed = BCrypt.Net.BCrypt.HashPassword(password);
            user.Password = hashed;
            try { await _userRepo.UpdateAsync(user); } catch { }
            return user;
        }

        return null;
    }

    private static string NormalizeRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role)) return "jobSeeker";
        var r = role.Trim().ToLowerInvariant();
        return r switch
        {
            "company" => "Company",
            "companyadmin" or "admin" => "Company",
            "jobseeker" or "job_seeker" or "job-seeker" => "jobSeeker",
            _ => char.ToUpperInvariant(role![0]) + role.Substring(1) // best-effort
        };
    }

    public string GenerateToken(User user)
    {
        var jwtKeyConfig = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not configured");
        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(jwtKeyConfig);
        }
        catch
        {
            keyBytes = Encoding.UTF8.GetBytes(jwtKeyConfig);
        }

        if (keyBytes.Length < 32)
            throw new InvalidOperationException("Jwt:Key must be at least 256 bits (32 bytes).");

        var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

        var normalizedRole = NormalizeRole(user.Role);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Role, normalizedRole)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresMinutes"] ?? "60")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}