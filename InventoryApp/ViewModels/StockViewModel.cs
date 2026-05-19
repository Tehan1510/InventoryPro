using InventoryApp.Data.Repositories;
using InventoryApp.Models;
using InventoryApp.Services;
using System.Collections.ObjectModel;

namespace InventoryApp.ViewModels;

public class StockViewModel : BaseViewModel
{
    private ObservableCollection<StockTransaction> _transactions = [];
    private ObservableCollection<Product> _products = [];
    private bool _isFormVisible, _isLoading;
    private Product? _formSelectedProduct;
    private TransactionType _formType = TransactionType.StockIn;
    private int _formQuantity = 1;
    private decimal _formUnitPrice;
    private string _formNotes = string.Empty, _statusMessage = string.Empty;

    public ObservableCollection<StockTransaction> Transactions { get => _transactions; set => SetProperty(ref _transactions, value); }
    public ObservableCollection<Product> Products { get => _products; set => SetProperty(ref _products, value); }
    public bool IsFormVisible { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public Product? FormSelectedProduct { get => _formSelectedProduct; set => SetProperty(ref _formSelectedProduct, value); }
    public TransactionType FormType { get => _formType; set => SetProperty(ref _formType, value); }
    public int FormQuantity { get => _formQuantity; set => SetProperty(ref _formQuantity, value); }
    public decimal FormUnitPrice { get => _formUnitPrice; set => SetProperty(ref _formUnitPrice, value); }
    public string FormNotes { get => _formNotes; set => SetProperty(ref _formNotes, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    // Populates the TransactionType ComboBox in XAML
    public IEnumerable<TransactionType> AllTypes { get; } = Enum.GetValues<TransactionType>();

    public RelayCommand ShowFormCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }

    private readonly StockTransactionRepository _stockRepo = new();
    private readonly ProductRepository _productRepo = new();

    public StockViewModel()
    {
        ShowFormCommand = new RelayCommand(ShowForm);
        SaveCommand = new RelayCommand(async () => await SaveAsync());
        CancelCommand = new RelayCommand(HideForm);
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            Transactions = new ObservableCollection<StockTransaction>(await _stockRepo.GetAllAsync());
            Products = new ObservableCollection<Product>(await _productRepo.GetAllAsync());
        }
        finally { IsLoading = false; }
    }

    private void ShowForm()
    {
        FormSelectedProduct = null; FormType = TransactionType.StockIn;
        FormQuantity = 1; FormUnitPrice = 0; FormNotes = string.Empty;
        StatusMessage = string.Empty; IsFormVisible = true;
    }

    private async Task SaveAsync()
    {
        if (FormSelectedProduct is null || FormQuantity <= 0)
        {
            StatusMessage = "Select a product and enter a valid quantity."; return;
        }
        if (FormType == TransactionType.StockOut && FormQuantity > FormSelectedProduct.Quantity)
        {
            StatusMessage = $"Insufficient stock. Available: {FormSelectedProduct.Quantity}"; return;
        }

        // 1. Record transaction
        await _stockRepo.AddAsync(new StockTransaction
        {
            ProductId = FormSelectedProduct.Id,
            Type = FormType,
            Quantity = FormQuantity,
            UnitPrice = FormUnitPrice,
            Notes = FormNotes,
            TransactionDate = DateTime.UtcNow,
            UserId = SessionManager.CurrentUser!.Id
        });

        // 2. Update product quantity
        var newQty = FormType switch
        {
            TransactionType.StockIn => FormSelectedProduct.Quantity + FormQuantity,
            TransactionType.StockOut => FormSelectedProduct.Quantity - FormQuantity,
            TransactionType.Adjustment => FormQuantity,
            _ => FormSelectedProduct.Quantity
        };

        await _productRepo.UpdateAsync(new Product
        {
            Id = FormSelectedProduct.Id,
            Name = FormSelectedProduct.Name,
            SKU = FormSelectedProduct.SKU,
            Description = FormSelectedProduct.Description,
            Price = FormSelectedProduct.Price,
            Quantity = newQty,
            LowStockThreshold = FormSelectedProduct.LowStockThreshold,
            CategoryId = FormSelectedProduct.CategoryId,
            SupplierId = FormSelectedProduct.SupplierId,
            CreatedAt = FormSelectedProduct.CreatedAt,
            IsActive = true
        });

        HideForm();
        await LoadAsync();
    }

    private void HideForm() { IsFormVisible = false; StatusMessage = string.Empty; }
}