using InventoryApp.ViewModels;
using System.Windows;

namespace InventoryApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var vm = new MainViewModel();
        vm.LogoutRequested += () => { new LoginWindow().Show(); Close(); };
        DataContext = vm;
    }
}