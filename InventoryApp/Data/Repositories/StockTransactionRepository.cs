using Microsoft.EntityFrameworkCore;
using InventoryApp.Models;

namespace InventoryApp.Data.Repositories;

public class StockTransactionRepository : BaseRepository<StockTransaction>
{
    public override async Task<IEnumerable<StockTransaction>> GetAllAsync()
    {
        using var ctx = new AppDbContext();
        return await ctx.StockTransactions
            .Include(t => t.Product)
            .Include(t => t.User)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockTransaction>> GetRecentAsync(int count)
    {
        using var ctx = new AppDbContext();
        return await ctx.StockTransactions
            .Include(t => t.Product)
            .Include(t => t.User)
            .OrderByDescending(t => t.TransactionDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockTransaction>> GetByProductIdAsync(int productId)
    {
        using var ctx = new AppDbContext();
        return await ctx.StockTransactions
            .Include(t => t.Product)
            .Include(t => t.User)
            .Where(t => t.ProductId == productId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }
}