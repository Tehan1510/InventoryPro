using InventoryApp.ViewModels;
using System.Windows;

namespace InventoryApp.Views;

public partial class LoginWindow : Window
{
    public LoginWindow() => InitializeComponent();

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = (LoginViewModel)DataContext;
        if (await vm.LoginAsync(PwdBox.Password))
        {
            new MainWindow().Show();
            Close();
        }
    }
}