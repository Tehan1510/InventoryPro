using Microsoft.EntityFrameworkCore;
using InventoryApp.Models;

namespace InventoryApp.Data.Repositories;

public class ProductRepository : BaseRepository<Product>
{
    // Count only active products for dashboard
    public override async Task<int> CountAsync()
    {
        using var ctx = new AppDbContext();
        return await ctx.Products.CountAsync(p => p.IsActive);
    }

    // Soft delete — keeps product so transaction history is preserved
    public override async Task DeleteAsync(int id)
    {
        using var ctx = new AppDbContext();
        var p = await ctx.Products.FindAsync(id);
        if (p is null) return;
        p.IsActive = false;
        await ctx.SaveChangesAsync();
    }

    // Override to include related Category and Supplier
    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        using var ctx = new AppDbContext();
        return await ctx.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchAsync(string term)
    {
        using var ctx = new AppDbContext();
        return await ctx.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.IsActive &&
                (p.Name.Contains(term) ||
                 p.SKU.Contains(term) ||
                 p.Description.Contains(term)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetLowStockAsync()
    {
        using var ctx = new AppDbContext();
        return await ctx.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.Quantity <= p.LowStockThreshold)
            .ToListAsync();
    }

    public async Task<bool> SkuExistsAsync(string sku, int excludeId = 0)
    {
        using var ctx = new AppDbContext();
        return await ctx.Products
            .AnyAsync(p => p.SKU.ToLower() == sku.ToLower() && p.Id != excludeId);
    }

    public async Task<int> GetLowStockCountAsync()
    {
        using var ctx = new AppDbContext();
        return await ctx.Products
            .CountAsync(p => p.IsActive && p.Quantity <= p.LowStockThreshold);
    }

    // Safe update — copies only scalar/FK fields to avoid navigation conflicts
    public override async Task UpdateAsync(Product p)
    {
        using var ctx = new AppDbContext();
        var existing = await ctx.Products.FindAsync(p.Id);
        if (existing is null) return;

        existing.Name = p.Name;
        existing.SKU = p.SKU;
        existing.Description = p.Description;
        existing.Price = p.Price;
        existing.Quantity = p.Quantity;
        existing.LowStockThreshold = p.LowStockThreshold;
        existing.CategoryId = p.CategoryId;
        existing.SupplierId = p.SupplierId;
        existing.IsActive = p.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await ctx.SaveChangesAsync();
    }
}