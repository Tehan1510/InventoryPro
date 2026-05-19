using InventoryApp.Data.Repositories;
using InventoryApp.Models;

namespace InventoryApp.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private int _totalProducts, _lowStockCount, _totalCategories, _totalSuppliers;
    private bool _isLoading;
    private IEnumerable<StockTransaction> _recentTransactions = [];

    public int TotalProducts { get => _totalProducts; set => SetProperty(ref _totalProducts, value); }
    public int LowStockCount { get => _lowStockCount; set => SetProperty(ref _lowStockCount, value); }
    public int TotalCategories { get => _totalCategories; set => SetProperty(ref _totalCategories, value); }
    public int TotalSuppliers { get => _totalSuppliers; set => SetProperty(ref _totalSuppliers, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }

    public IEnumerable<StockTransaction> RecentTransactions
    {
        get => _recentTransactions;
        set => SetProperty(ref _recentTransactions, value);
    }

    public RelayCommand RefreshCommand { get; }

    public DashboardViewModel()
    {
        RefreshCommand = new RelayCommand(async () => await LoadAsync());
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            TotalProducts = await new ProductRepository().CountAsync();
            LowStockCount = await new ProductRepository().GetLowStockCountAsync();
            TotalCategories = await new CategoryRepository().CountAsync();
            TotalSuppliers = await new SupplierRepository().CountAsync();
            RecentTransactions = await new StockTransactionRepository().GetRecentAsync(5);
        }
        finally { IsLoading = false; }
    }
}