using System.IO;
using Microsoft.EntityFrameworkCore;
using InventoryApp.Models;

namespace InventoryApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<StockTransaction> StockTransactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Store DB in %LocalAppData%/InventoryApp/
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(folder, "InventoryApp");
        Directory.CreateDirectory(appFolder);
        var dbPath = Path.Combine(appFolder, "inventory.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed default categories on first run
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and accessories" },
            new Category { Id = 2, Name = "Office Supplies", Description = "Stationery and office equipment" },
            new Category { Id = 3, Name = "Furniture", Description = "Office and home furniture" }
        );
    }
}