using JobsAPI.Models;
using JobsAPI.Repositories;

namespace JobsAPI.Services;

public interface ICompanyService
{
    Task<IEnumerable<Company>> GetAllAsync();
    Task<Company?> GetAsync(int id);
    Task<Company> CreateAsync(Company c);
    Task<Company?> UpdateAsync(Company c);
    Task<bool> DeleteAsync(int id);
}

public class CompanyService : ICompanyService
{
    private readonly IRepository<Company> _repo;
    public CompanyService(IRepository<Company> repo) => _repo = repo;

    public Task<Company> CreateAsync(Company c) => _repo.AddAsync(c);
    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    public Task<IEnumerable<Company>> GetAllAsync() => _repo.GetAllAsync();
    public Task<Company?> GetAsync(int id) => _repo.GetByIdAsync(id);
    public Task<Company?> UpdateAsync(Company c) => _repo.UpdateAsync(c);
}