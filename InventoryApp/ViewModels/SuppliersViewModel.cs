using InventoryApp.Data.Repositories;
using InventoryApp.Models;
using System.Collections.ObjectModel;

namespace InventoryApp.ViewModels;

public class SuppliersViewModel : BaseViewModel
{
    private ObservableCollection<Supplier> _suppliers = [];
    private Supplier? _selectedSupplier;
    private bool _isFormVisible, _isEditing, _isLoading;
    private string _formName = string.Empty, _formContact = string.Empty;
    private string _formEmail = string.Empty, _formPhone = string.Empty, _formAddress = string.Empty;
    private string _statusMessage = string.Empty;

    public ObservableCollection<Supplier> Suppliers { get => _suppliers; set => SetProperty(ref _suppliers, value); }
    public Supplier? SelectedSupplier { get => _selectedSupplier; set => SetProperty(ref _selectedSupplier, value); }
    public bool IsFormVisible { get => _isFormVisible; set => SetProperty(ref _isFormVisible, value); }
    public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }
    public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
    public string FormName { get => _formName; set => SetProperty(ref _formName, value); }
    public string FormContact { get => _formContact; set => SetProperty(ref _formContact, value); }
    public string FormEmail { get => _formEmail; set => SetProperty(ref _formEmail, value); }
    public string FormPhone { get => _formPhone; set => SetProperty(ref _formPhone, value); }
    public string FormAddress { get => _formAddress; set => SetProperty(ref _formAddress, value); }
    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }
    public string FormTitle => IsEditing ? "Edit Supplier" : "Add Supplier";

    public RelayCommand ShowAddFormCommand { get; }
    public RelayCommand EditCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }

    private readonly SupplierRepository _repo = new();

    public SuppliersViewModel()
    {
        ShowAddFormCommand = new RelayCommand(ShowAddForm);
        EditCommand = new RelayCommand(ShowEditForm, () => SelectedSupplier is not null);
        DeleteCommand = new RelayCommand(async () => await DeleteAsync(), () => SelectedSupplier is not null);
        SaveCommand = new RelayCommand(async () => await SaveAsync());
        CancelCommand = new RelayCommand(HideForm);
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        IsLoading = true;
        try { Suppliers = new ObservableCollection<Supplier>(await _repo.GetAllAsync()); }
        finally { IsLoading = false; }
    }

    private void ShowAddForm()
    {
        IsEditing = false;
        FormName = FormContact = FormEmail = FormPhone = FormAddress = string.Empty;
        StatusMessage = string.Empty; IsFormVisible = true;
    }

    private void ShowEditForm()
    {
        if (SelectedSupplier is null) return;
        IsEditing = true;
        FormName = SelectedSupplier.Name; FormContact = SelectedSupplier.ContactPerson;
        FormEmail = SelectedSupplier.Email; FormPhone = SelectedSupplier.Phone; FormAddress = SelectedSupplier.Address;
        StatusMessage = string.Empty; IsFormVisible = true;
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FormName)) { StatusMessage = "Supplier name is required."; return; }

        if (IsEditing && SelectedSupplier is not null)
        {
            SelectedSupplier.Name = FormName; SelectedSupplier.ContactPerson = FormContact;
            SelectedSupplier.Email = FormEmail; SelectedSupplier.Phone = FormPhone; SelectedSupplier.Address = FormAddress;
            await _repo.UpdateAsync(SelectedSupplier);
        }
        else
        {
            await _repo.AddAsync(new Supplier
            {
                Name = FormName,
                ContactPerson = FormContact,
                Email = FormEmail,
                Phone = FormPhone,
                Address = FormAddress
            });
        }
        HideForm(); await LoadAsync();
    }

    private async Task DeleteAsync()
    {
        if (SelectedSupplier is null) return;
        await _repo.DeleteAsync(SelectedSupplier.Id);
        await LoadAsync();
    }

    private void HideForm() { IsFormVisible = false; StatusMessage = string.Empty; }
}