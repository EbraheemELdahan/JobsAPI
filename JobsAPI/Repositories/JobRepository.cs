using JobsAPI.Models;

namespace JobsAPI.Repositories;

public class JobRepository : IRepository<Job>
{
    private readonly JsonFileStore _store;
    private List<Job> _items = new();

    public JobRepository(JsonFileStore store)
    {
        _store = store;
        _items = _store.ReadArrayAsync<Job>("jobs").GetAwaiter().GetResult();
    }

    public Task<Job> AddAsync(Job entity)
    {
        entity.Id = (_items.Any() ? _items.Max(x => x.Id) : 0) + 1;
        _items.Add(entity);
        _store.WriteArrayAsync("jobs", _items).GetAwaiter().GetResult();
        return Task.FromResult(entity);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var removed = _items.RemoveAll(x => x.Id == id) > 0;
        _store.WriteArrayAsync("jobs", _items).GetAwaiter().GetResult();
        return Task.FromResult(removed);
    }

    public Task<IEnumerable<Job>> GetAllAsync() => Task.FromResult<IEnumerable<Job>>(_items);

    public Task<Job?> GetByIdAsync(int id) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

    public Task<Job?> UpdateAsync(Job entity)
    {
        var idx = _items.FindIndex(x => x.Id == entity.Id);
        if (idx == -1) return Task.FromResult<Job?>(null);
        _items[idx] = entity;
        _store.WriteArrayAsync("jobs", _items).GetAwaiter().GetResult();
        return Task.FromResult<Job?>(entity);
    }
}