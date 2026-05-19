using InventoryApp.Data;
using InventoryApp.Data.Repositories;
using InventoryApp.Services;
using InventoryApp.Views;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace InventoryApp;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Creates inventory.db and all tables on first run
        using var ctx = new AppDbContext();
        await ctx.Database.EnsureCreatedAsync();

        // Seed admin account on first run
        if (!await ctx.Users.AnyAsync())
            await new AuthService(new UserRepository())
                .RegisterAsync("admin", "admin@inventory.com", "Admin@123", "Admin");

        new LoginWindow().Show();
    }
}