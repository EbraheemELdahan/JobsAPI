using JobsAPI.Models;

namespace JobsAPI.Repositories;

public class UserRepository : IRepository<User>
{
    private readonly JsonFileStore _store;
    private List<User> _users = new();

    public UserRepository(JsonFileStore store)
    {
        _store = store;
        _users = _store.ReadArrayAsync<User>("users").GetAwaiter().GetResult();
    }

    public Task<User> AddAsync(User entity)
    {
        entity.Id = (_users.Any() ? _users.Max(u => u.Id) : 0) + 1;
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

    public Task<IEnumerable<User>> GetAllAsync() => Task.FromResult<IEnumerable<User>>(_users);

    public Task<User?> GetByIdAsync(int id) => Task.FromResult(_users.FirstOrDefault(u => u.Id == id));

    public Task<User?> UpdateAsync(User entity)
    {
        var idx = _users.FindIndex(u => u.Id == entity.Id);
        if (idx == -1) return Task.FromResult<User?>(null);
        _users[idx] = entity;
        _store.WriteArrayAsync("users", _users).GetAwaiter().GetResult();
        return Task.FromResult<User?>(entity);
    }
}