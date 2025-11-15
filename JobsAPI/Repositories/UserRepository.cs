using System.Linq;
using JobsAPI.Models;

namespace JobsAPI.Repositories;

public class UserRepository : IRepository<User>
{
    private readonly JsonFileStore _store;
    private List<User> _users = new();
    private List<Company> _companies = new();

    public UserRepository(JsonFileStore store)
    {
        _store = store;
        _users = _store.ReadArrayAsync<User>("users").GetAwaiter().GetResult();
        _companies = _store.ReadArrayAsync<Company>("companies").GetAwaiter().GetResult();

        // populate company navigation for loaded users
        PopulateCompanies(_users);
    }

    private void PopulateCompanies(IEnumerable<User> users)
    {
        if (_companies is null || _companies.Count == 0) return;

        foreach (var u in users)
        {
            if (u.CompanyId == null)
            {
                u.Company = null;
                continue;
            }

            var c = _companies.FirstOrDefault(x => x.Id == u.CompanyId);
            if (c == null)
            {
                u.Company = null;
                continue;
            }

            // only include the requested fields to avoid returning full company object
            u.Company = new Company
            {
                Id = c.Id,
                Name = c.Name,
                Logo = c.Logo
            };
        }
    }

    public Task<User> AddAsync(User entity)
    {
        entity.Id = (_users.Any() ? _users.Max(u => u.Id) : 0) + 1;
        // set lightweight company for return if CompanyId provided
        if (entity.CompanyId != null)
        {
            var c = _companies.FirstOrDefault(x => x.Id == entity.CompanyId);
            if (c != null)
                entity.Company = new Company { Id = c.Id, Name = c.Name, Logo = c.Logo };
        }

        _users.Add(entity);
        _store.WriteArrayAsync("users", _users).GetAwaiter().GetResult();
        return Task.FromResult(entity);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var removed = _users.RemoveAll(u => u.Id == id) > 0;
        _store.WriteArrayAsync("users", _users).GetAwaiter().GetResult();
        return Task.FromResult(removed);
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        // ensure navigation is populated each call in case companies changed
        PopulateCompanies(_users);
        return Task.FromResult<IEnumerable<User>>(_users);
    }

    public Task<User?> GetByIdAsync(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            // populate company for this user on demand
            if (user.CompanyId != null)
            {
                var c = _companies.FirstOrDefault(x => x.Id == user.CompanyId);
                user.Company = c == null ? null : new Company { Id = c.Id, Name = c.Name, Logo = c.Logo };
            }
            else
            {
                user.Company = null;
            }
        }

        return Task.FromResult(user);
    }

    public Task<User?> UpdateAsync(User entity)
    {
        var idx = _users.FindIndex(u => u.Id == entity.Id);
        if (idx == -1) return Task.FromResult<User?>(null);

        // update lightweight company navigation
        if (entity.CompanyId != null)
        {
            var c = _companies.FirstOrDefault(x => x.Id == entity.CompanyId);
            entity.Company = c == null ? null : new Company { Id = c.Id, Name = c.Name, Logo = c.Logo };
        }
        else
        {
            entity.Company = null;
        }

        _users[idx] = entity;
        _store.WriteArrayAsync("users", _users).GetAwaiter().GetResult();
        return Task.FromResult<User?>(entity);
    }
}