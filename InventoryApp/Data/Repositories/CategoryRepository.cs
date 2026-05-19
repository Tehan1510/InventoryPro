using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Data.Repositories;

public class CategoryRepository : BaseRepository<InventoryApp.Models.Category>
{
    // Safe update — avoids navigation property conflicts
    public override async Task UpdateAsync(InventoryApp.Models.Category c)
    {
        using var ctx = new AppDbContext();
        var e = await ctx.Categories.FindAsync(c.Id);
        if (e is null) return;
        e.Name = c.Name;
        e.Description = c.Description;
        await ctx.SaveChangesAsync();
    }

    public async Task<bool> NameExistsAsync(string name, int excludeId = 0)
    {
        using var ctx = new AppDbContext();
        return await ctx.Categories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != excludeId);
    }

    public async Task<bool> HasProductsAsync(int categoryId)
    {
        using var ctx = new AppDbContext();
        return await ctx.Products.AnyAsync(p => p.CategoryId == categoryId && p.IsActive);
    }
}