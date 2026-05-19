using InventoryApp.Models;

namespace InventoryApp.Services;

// Holds the currently logged-in user for the entire app session
public static class SessionManager
{
    public static User? CurrentUser { get; private set; }
    public static bool IsLoggedIn => CurrentUser is not null;
    public static bool IsAdmin => CurrentUser?.Role == "Admin";

    public static void Login(User user) => CurrentUser = user;
    public static void Logout() => CurrentUser = null;
}