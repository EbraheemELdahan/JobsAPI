using JobsAPI.Models;

namespace JobsAPI.Repositories;

public class ApplicationRepository : IRepository<Application>
{
    private readonly JsonFileStore _store;
    private List<Application> _items = new();

    public ApplicationRepository(JsonFileStore store)
    {
        _store = store;
        _items = _store.ReadArrayAsync<Application>("applications").GetAwaiter().GetResult();
    }

    public Task<Application> AddAsync(Application entity)
    {
        entity.Id = (_items.Any() ? _items.Max(x => x.Id) : 0) + 1;
        if (entity.AppliedAt == null) entity.AppliedAt = DateTime.UtcNow;
        _items.Add(entity);
        _store.WriteArrayAsync("applications", _items).GetAwaiter().GetResult();
        return Task.FromResult(entity);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var removed = _items.RemoveAll(x => x.Id == id) > 0;
        _store.WriteArrayAsync("applications", _items).GetAwaiter().GetResult();
        return Task.FromResult(removed);
    }

    public Task<IEnumerable<Application>> GetAllAsync() => Task.FromResult<IEnumerable<Application>>(_items);

    public Task<Application?> GetByIdAsync(int id) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

    public Task<Application?> UpdateAsync(Application entity)
    {
        var idx = _items.FindIndex(x => x.Id == entity.Id);
        if (idx == -1) return Task.FromResult<Application?>(null);
        _items[idx] = entity;
        _store.WriteArrayAsync("applications", _items).GetAwaiter().GetResult();
        return Task.FromResult<Application?>(entity);
    }
}