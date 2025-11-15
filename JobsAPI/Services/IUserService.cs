using JobsAPI.Models;
using JobsAPI.Repositories;

namespace JobsAPI.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetAsync(int id);
    Task<User> CreateAsync(User u);
    Task<User?> UpdateAsync(User u);
    Task<bool> DeleteAsync(int id);
}

public class UserService : IUserService
{
    private readonly IRepository<User> _repo;
    public UserService(IRepository<User> repo) => _repo = repo;

    private static UserDto ToDto(User u)
    {
        return new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            // Password intentionally omitted from output; kept in DTO so internal code can access if needed
            Password = u.Password,
            Role = u.Role,
            Phone = u.Phone,
            CompanyId = u.CompanyId,
            Company = u.Company == null ? null : new CompanyLiteDto
            {
                Id = u.Company.Id,
                Name = u.Company.Name,
                Logo = u.Company.Logo
            }
        };
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _repo.GetAllAsync();
        return users.Select(ToDto);
    }

    public async Task<UserDto?> GetAsync(int id)
    {
        var u = await _repo.GetByIdAsync(id);
        return u == null ? null : ToDto(u);
    }

    public Task<User> CreateAsync(User u) => _repo.AddAsync(u);
    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    public Task<User?> UpdateAsync(User u) => _repo.UpdateAsync(u);
}