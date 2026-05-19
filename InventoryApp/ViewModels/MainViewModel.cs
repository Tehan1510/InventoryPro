using InventoryApp.Services;

namespace InventoryApp.ViewModels;

public class MainViewModel : BaseViewModel
{
    private BaseViewModel _currentViewModel;

    public BaseViewModel CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public string CurrentUsername => SessionManager.CurrentUser?.Username ?? string.Empty;
    public string CurrentRole => SessionManager.CurrentUser?.Role ?? string.Empty;

    // MainWindow.xaml.cs listens to this to switch back to LoginWindow
    public event Action? LogoutRequested;

    public RelayCommand NavDashboardCommand { get; }
    public RelayCommand NavProductsCommand { get; }
    public RelayCommand NavCategoriesCommand { get; }
    public RelayCommand NavSuppliersCommand { get; }
    public RelayCommand NavStockCommand { get; }
    public RelayCommand LogoutCommand { get; }

    public MainViewModel()
    {
        NavDashboardCommand = new RelayCommand(() => CurrentViewModel = new DashboardViewModel());
        NavProductsCommand = new RelayCommand(() => CurrentViewModel = new ProductsViewModel());
        NavCategoriesCommand = new RelayCommand(() => CurrentViewModel = new CategoriesViewModel());
        NavSuppliersCommand = new RelayCommand(() => CurrentViewModel = new SuppliersViewModel());
        NavStockCommand = new RelayCommand(() => CurrentViewModel = new StockViewModel());
        LogoutCommand = new RelayCommand(Logout);

        _currentViewModel = new DashboardViewModel(); // start here
    }

    private void Logout()
    {
        SessionManager.Logout();
        LogoutRequested?.Invoke();
    }
}