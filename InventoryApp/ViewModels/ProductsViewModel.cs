using InventoryApp.Data.Repositories;
using InventoryApp.Models;
using System.Collections.ObjectModel;

namespace InventoryApp.ViewModels;

public class ProductsViewModel : BaseViewModel
{
    // List state
    private ObservableCollection<Product> _products = [];
    private Product? _selectedProduct;
    private bool _isLoading;
    private string _searchText = string.Empty;

    // Form state
    private bool _isFormVisible, _isEditing;
    private string _formName = string.Empty, _formSKU = string.Empty, _formDescription = string.Empty;
    private decimal _formPrice;
    private int _formQuantity, _formLowStockThreshold = 10;
    private Category? _formSelectedCategory;
    private Supplier? _formSelectedSupplier;
    private string _statusMessage = string.Empty;

    // ComboBox sources
    private ObservableCollection<Category> _categories = [];
    private ObservableCollection<Supplier> _suppliers = [];

    public ObservableCollection<Product> Products { get => _products; set => SetProperty(ref _products, value); }
    public ObservableCollection<Category> Categories { get => _categories; set => SetProperty(ref _categories, value); }
    public ObservableCollection<Supplier> Suppliers { get => _suppliers; set => SetProperty(ref _suppliers, value); }
    public Product? SelectedProduct { get => _selectedProduct; set => SetProperty(ref _selectedProduct, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public bool IsFormVisible { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string FormTitle => IsEditing ? "Edit Product" : "Add Product";

    public string SearchText
    {
        get => _searchText;
        set { SetProperty(ref _searchText, value); _ = SearchAsync(); }
    }

    public string FormName { get => _formName; set => SetProperty(ref _formName, value); }
    public string FormSKU { get => _formSKU; set => SetProperty(ref _formSKU, value); }
    public string FormDescription { get => _formDescription; set => SetProperty(ref _formDescription, value); }
    public decimal FormPrice { get => _formPrice; set => SetProperty(ref _formPrice, value); }
    public int FormQuantity { get => _formQuantity; set => SetProperty(ref _formQuantity, value); }
    public int FormLowStockThreshold { get => _formLowStockThreshold; set => SetProperty(ref _formLowStockThreshold, value); }
    public Category? FormSelectedCategory { get => _formSelectedCategory; set => SetProperty(ref _formSelectedCategory, value); }
    public Supplier? FormSelectedSupplier { get => _formSelectedSupplier; set => SetProperty(ref _formSelectedSupplier, value); }

    public RelayCommand ShowAddFormCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }

    private readonly ProductRepository _productRepo = new();
    private readonly CategoryRepository _categoryRepo = new();
    private readonly SupplierRepository _supplierRepo = new();

    public ProductsViewModel()
    {
        ShowAddFormCommand = new RelayCommand(ShowAddForm);
        EditCommand = new RelayCommand(ShowEditForm, () => SelectedProduct is not null);
        DeleteCommand = new RelayCommand(async () => await DeleteAsync(), () => SelectedProduct is not null);
        SaveCommand = new RelayCommand(async () => await SaveAsync());
        CancelCommand = new RelayCommand(HideForm);
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            Products = new ObservableCollection<Product>(await _productRepo.GetAllAsync());
            Categories = new ObservableCollection<Category>(await _categoryRepo.GetAllAsync());
            Suppliers = new ObservableCollection<Supplier>(await _supplierRepo.GetAllAsync());
        }
        finally { IsLoading = false; }
    }

    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText)) { await LoadAsync(); return; }
        Products = new ObservableCollection<Product>(await _productRepo.SearchAsync(SearchText));
    }

    private void ShowAddForm()
    {
        IsEditing = false;
        FormName = FormSKU = FormDescription = string.Empty;
        FormPrice = 0; FormQuantity = 0; FormLowStockThreshold = 10;
        FormSelectedCategory = null; FormSelectedSupplier = null;
        StatusMessage = string.Empty;
        IsFormVisible = true;
    }

    private void ShowEditForm()
    {
        if (SelectedProduct is null) return;
        IsEditing = true;
        FormName = SelectedProduct.Name;
        FormSKU = SelectedProduct.SKU;
        FormDescription = SelectedProduct.Description;
        FormPrice = SelectedProduct.Price;
        FormQuantity = SelectedProduct.Quantity;
        FormLowStockThreshold = SelectedProduct.LowStockThreshold;
        FormSelectedCategory = Categories.FirstOrDefault(c => c.Id == SelectedProduct.CategoryId);
        FormSelectedSupplier = Suppliers.FirstOrDefault(s => s.Id == SelectedProduct.SupplierId);
        StatusMessage = string.Empty;
        IsFormVisible = true;
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FormName) || FormSelectedCategory is null)
        {
            StatusMessage = "Name and Category are required."; return;
        }

        if (IsEditing && SelectedProduct is not null)
        {
            await _productRepo.UpdateAsync(new Product
            {
                Id = SelectedProduct.Id,
                Name = FormName,
                SKU = FormSKU,
                Description = FormDescription,
                Price = FormPrice,
                Quantity = FormQuantity,
                LowStockThreshold = FormLowStockThreshold,
                CategoryId = FormSelectedCategory.Id,
                SupplierId = FormSelectedSupplier?.Id,
                CreatedAt = SelectedProduct.CreatedAt,
                IsActive = true
            });
        }
        else
        {
            await _productRepo.AddAsync(new Product
            {
                Name = FormName,
                SKU = FormSKU,
                Description = FormDescription,
                Price = FormPrice,
                Quantity = FormQuantity,
                LowStockThreshold = FormLowStockThreshold,
                CategoryId = FormSelectedCategory.Id,
                SupplierId = FormSelectedSupplier?.Id
            });
        }
        HideForm();
        await LoadAsync();
    }

    private async Task DeleteAsync()
    {
        if (SelectedProduct is null) return;
        await _productRepo.DeleteAsync(SelectedProduct.Id); // soft delete
        await LoadAsync();
    }

    private void HideForm() { IsFormVisible = false; StatusMessage = string.Empty; }
}