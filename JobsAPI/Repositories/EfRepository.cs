using System.Linq.Expressions;
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

    private IQueryable<T> BuildQueryWithIncludes()
    {
        IQueryable<T> query = _set.AsQueryable();

        // If this entity has a "Company" navigation, include it so returned Users carry Company data
        var entityType = _db.Model.FindEntityType(typeof(T));
        if (entityType != null && entityType.FindNavigation("Company") != null)
        {
            query = query.Include("Company");
        }

        return query;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var query = BuildQueryWithIncludes();
        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        // Prefer query approach to allow Includes to work
        var keyProperty = typeof(T).GetProperty("Id");
        if (keyProperty == null)
        {
            // fallback to FindAsync if no Id property
            return await _set.FindAsync(id);
        }

        var query = BuildQueryWithIncludes();

        // Build lambda: e => e.Id == id
        var param = Expression.Parameter(typeof(T), "e");
        var left = Expression.Property(param, keyProperty);
        var right = Expression.Constant(Convert.ChangeType(id, keyProperty.PropertyType));
        var body = Expression.Equal(left, right);
        var lambda = Expression.Lambda<Func<T, bool>>(body, param);

        return await query.FirstOrDefaultAsync(lambda);
    }

    public async Task<T?> UpdateAsync(T entity)
    {
        _set.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}