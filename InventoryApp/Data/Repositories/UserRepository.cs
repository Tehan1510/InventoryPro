using Microsoft.EntityFrameworkCore;
using InventoryApp.Models;

namespace InventoryApp.Data.Repositories;

public class UserRepository : BaseRepository<User>
{
    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var ctx = new AppDbContext();
        return await ctx.Users
            .FirstOrDefaultAsync(u =>
                u.Username.ToLower() == username.ToLower() && u.IsActive);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        using var ctx = new AppDbContext();
        return await ctx.Users
            .AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }
}