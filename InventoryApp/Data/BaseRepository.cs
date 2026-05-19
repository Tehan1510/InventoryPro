using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Data;

public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        using var ctx = new AppDbContext();
        return await ctx.Set<T>().ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        using var ctx = new AppDbContext();
        return await ctx.Set<T>().FindAsync(id);
    }

    public virtual async Task AddAsync(T entity)
    {
        using var ctx = new AppDbContext();
        await ctx.Set<T>().AddAsync(entity);
        await ctx.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        using var ctx = new AppDbContext();
        ctx.Set<T>().Update(entity);
        await ctx.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        using var ctx = new AppDbContext();
        var entity = await ctx.Set<T>().FindAsync(id);
        if (entity is not null)
        {
            ctx.Set<T>().Remove(entity);
            await ctx.SaveChangesAsync();
        }
    }

    public virtual async Task<int> CountAsync()
    {
        using var ctx = new AppDbContext();
        return await ctx.Set<T>().CountAsync();
    }
}