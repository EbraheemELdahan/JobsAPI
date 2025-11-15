using JobsAPI.Models;
using JobsAPI.Repositories;

namespace JobsAPI.Services;

public interface IApplicationService
{
    Task<IEnumerable<Application>> GetAllAsync();
    Task<Application?> GetAsync(int id);
    Task<Application> CreateAsync(Application a);
    Task<Application?> UpdateAsync(Application a);
    Task<bool> DeleteAsync(int id);
}

public class ApplicationService : IApplicationService
{
    private readonly IRepository<Application> _repo;
    public ApplicationService(IRepository<Application> repo) => _repo = repo;

    public Task<Application> CreateAsync(Application a) => _repo.AddAsync(a);
    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    public Task<IEnumerable<Application>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Application?> GetAsync(int id) => _repo.GetByIdAsync(id);
    public Task<Application?> UpdateAsync(Application a) => _repo.UpdateAsync(a);
}