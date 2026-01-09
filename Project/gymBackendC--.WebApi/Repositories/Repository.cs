using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace gymBackendC__.WebApi.Repositories;

public class Repository<T>(DbContext dbContext) : IRepository<T> where T : class
{
    protected readonly DbSet<T> _dbSet = dbContext.Set<T>();
    
    public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();
    
    public async ValueTask<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    
    public async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }
}