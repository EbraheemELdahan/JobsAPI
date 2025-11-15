using JobsAPI.Models;

namespace JobsAPI.Repositories;

public class CompanyRepository : IRepository<Company>
{
    private readonly JsonFileStore _store;
    private List<Company> _items = new();

    public CompanyRepository(JsonFileStore store)
    {
        _store = store;
        _items = _store.ReadArrayAsync<Company>("companies").GetAwaiter().GetResult();
    }

    public Task<Company> AddAsync(Company entity)
    {
        entity.Id = (_items.Any() ? _items.Max(x => x.Id) : 0) + 1;
        _items.Add(entity);
        _store.WriteArrayAsync("companies", _items).GetAwaiter().GetResult();
        return Task.FromResult(entity);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var removed = _items.RemoveAll(x => x.Id == id) > 0;
        _store.WriteArrayAsync("companies", _items).GetAwaiter().GetResult();
        return Task.FromResult(removed);
    }

    public Task<IEnumerable<Company>> GetAllAsync() => Task.FromResult<IEnumerable<Company>>(_items);

    public Task<Company?> GetByIdAsync(int id) => Task.FromResult(_items.FirstOrDefault(x => x.Id == id));

    public Task<Company?> UpdateAsync(Company entity)
    {
        var idx = _items.FindIndex(x => x.Id == entity.Id);
        if (idx == -1) return Task.FromResult<Company?>(null);
        _items[idx] = entity;
        _store.WriteArrayAsync("companies", _items).GetAwaiter().GetResult();
        return Task.FromResult<Company?>(entity);
    }
}