namespace InventoryApp.Data.Repositories;

public class SupplierRepository : BaseRepository<InventoryApp.Models.Supplier>
{
    public override async Task UpdateAsync(InventoryApp.Models.Supplier s)
    {
        using var ctx = new AppDbContext();
        var e = await ctx.Suppliers.FindAsync(s.Id);
        if (e is null) return;
        e.Name = s.Name; e.ContactPerson = s.ContactPerson;
        e.Email = s.Email; e.Phone = s.Phone; e.Address = s.Address;
        await ctx.SaveChangesAsync();
    }
}