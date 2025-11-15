using JobsAPI.Models;
using JobsAPI.Repositories;

namespace JobsAPI.Services;

public interface IJobService
{
    Task<IEnumerable<Job>> GetAllAsync();
    Task<Job?> GetAsync(int id);
    Task<Job> CreateAsync(Job j);
    Task<Job?> UpdateAsync(Job j);
    Task<bool> DeleteAsync(int id);
}

public class JobService : IJobService
{
    private readonly IRepository<Job> _repo;
    public JobService(IRepository<Job> repo) => _repo = repo;

    public Task<Job> CreateAsync(Job j) => _repo.AddAsync(j);
    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    public Task<IEnumerable<Job>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Job?> GetAsync(int id) => _repo.GetByIdAsync(id);
    public Task<Job?> UpdateAsync(Job j) => _repo.UpdateAsync(j);
}