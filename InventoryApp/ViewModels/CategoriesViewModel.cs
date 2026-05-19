using InventoryApp.Data.Repositories;
using InventoryApp.Models;
using System.Collections.ObjectModel;

namespace InventoryApp.ViewModels;

public class CategoriesViewModel : BaseViewModel
{
    private ObservableCollection<Category> _categories = [];
    private Category? _selectedCategory;
    private bool _isFormVisible, _isEditing, _isLoading;
    private string _formName = string.Empty, _formDescription = string.Empty;
    private string _statusMessage = string.Empty;

    public ObservableCollection<Category> Categories { get => _categories; set => SetProperty(ref _categories, value); }
    public Category? SelectedCategory { get => _selectedCategory; set => SetProperty(ref _selectedCategory, value); }
    public bool IsFormVisible { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string FormName { get => _formName; set => SetProperty(ref _formName, value); }
    public string FormDescription { get => _formDescription; set => SetProperty(ref _formDescription, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string FormTitle => IsEditing ? "Edit Category" : "Add Category";

    public RelayCommand ShowAddFormCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }

    private readonly CategoryRepository _repo = new();

    public CategoriesViewModel()
    {
        ShowAddFormCommand = new RelayCommand(ShowAddForm);
        EditCommand = new RelayCommand(ShowEditForm, () => SelectedCategory is not null);
        DeleteCommand = new RelayCommand(async () => await DeleteAsync(), () => SelectedCategory is not null);
        SaveCommand = new RelayCommand(async () => await SaveAsync());
        CancelCommand = new RelayCommand(HideForm);
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        IsLoading = true;
        try { Categories = new ObservableCollection<Category>(await _repo.GetAllAsync()); }
        finally { IsLoading = false; }
    }

    private void ShowAddForm()
    {
        IsEditing = false;
        FormName = FormDescription = string.Empty;
        StatusMessage = string.Empty;
        IsFormVisible = true;
    }

    private void ShowEditForm()
    {
        if (SelectedCategory is null) return;
        IsEditing = true;
        FormName = SelectedCategory.Name; FormDescription = SelectedCategory.Description;
        StatusMessage = string.Empty; IsFormVisible = true;
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FormName)) { StatusMessage = "Name is required."; return; }
        if (await _repo.NameExistsAsync(FormName, IsEditing ? SelectedCategory!.Id : 0))
        {
            StatusMessage = "A category with this name already exists."; return;
        }

        if (IsEditing && SelectedCategory is not null)
        {
            SelectedCategory.Name = FormName;
            SelectedCategory.Description = FormDescription;
            await _repo.UpdateAsync(SelectedCategory);
        }
        else { await _repo.AddAsync(new Category { Name = FormName, Description = FormDescription }); }

        HideForm();
        await LoadAsync();
    }

    private async Task DeleteAsync()
    {
        if (SelectedCategory is null) return;
        if (await _repo.HasProductsAsync(SelectedCategory.Id))
        {
            StatusMessage = "Cannot delete: this category has active products."; return;
        }
        await _repo.DeleteAsync(SelectedCategory.Id);
        await LoadAsync();
    }

    private void HideForm() { IsFormVisible = false; StatusMessage = string.Empty; }
}