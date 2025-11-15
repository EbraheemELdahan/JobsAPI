using JobsAPI.Models;
using JobsAPI.Repositories;

namespace JobsAPI.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetAsync(int id);
    Task<User> CreateAsync(User u);
    Task<User?> UpdateAsync(User u);
    Task<bool> DeleteAsync(int id);
}

public class UserService : IUserService
{
    private readonly IRepository<User> _repo;
    public UserService(IRepository<User> repo) => _repo = repo;

    public Task<User> CreateAsync(User u) => _repo.AddAsync(u);
    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    public Task<IEnumerable<User>> GetAllAsync() => _repo.GetAllAsync();
    public Task<User?> GetAsync(int id) => _repo.GetByIdAsync(id);
    public Task<User?> UpdateAsync(User u) => _repo.UpdateAsync(u);
}