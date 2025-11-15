using JobsAPI.Models;

namespace JobsAPI.Services;

public interface IAuthService
{
    Task<User?> ValidateCredentialsAsync(string email, string password);
    string GenerateToken(User user);
}