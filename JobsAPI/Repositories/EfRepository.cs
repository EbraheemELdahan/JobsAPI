using Microsoft.EntityFrameworkCore;
using JobsAPI.Data;

namespace JobsAPI.Repositories;

public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    private readonly DbSet<T> _set;

    public EfRepository(ApplicationDbContext db)
    {
        _db = db;
        _set = _db.Set<T>();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _set.AddAsync(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var keyProperty = typeof(T).GetProperty("Id");
        if (keyProperty == null) return false;
        var entity = await _set.FindAsync(id);
        if (entity == null) return false;
        _set.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _set.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _set.FindAsync(id);
    }

    public async Task<T?> UpdateAsync(T entity)
    {
        _set.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}