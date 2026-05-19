using InventoryApp.Data.Repositories;
using InventoryApp.Services;

namespace InventoryApp.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private string _username = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    public string Username { get => _username; set => SetProperty(ref _username, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool IsNotLoading => !IsLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set { SetProperty(ref _isLoading, value); OnPropertyChanged(nameof(IsNotLoading)); }
    }
    // Called from code-behind — WPF PasswordBox can't be bound in XAML for security reasons
    public async Task<bool> LoginAsync(string password)
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        try
        {
            var (success, message, user) =
                await new AuthService(new UserRepository()).LoginAsync(Username, password);

            if (success && user is not null) { SessionManager.Login(user); return true; }
            ErrorMessage = message;
            return false;
        }
        finally { IsLoading = false; }
    }
}