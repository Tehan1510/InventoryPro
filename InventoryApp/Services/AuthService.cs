using InventoryApp.Data.Repositories;
using InventoryApp.Models;

namespace InventoryApp.Services;

public class AuthService
{
    private readonly UserRepository _users;

    public AuthService(UserRepository users) => _users = users;

    public async Task<(bool Success, string Message, User? User)> LoginAsync(
        string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return (false, "Username and password are required.", null);

        var user = await _users.GetByUsernameAsync(username);
        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, "Invalid username or password.", null);

        return (true, "Login successful.", user);
    }

    public async Task<(bool Success, string Message)> RegisterAsync(
        string username, string email, string password, string role = "User")
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return (false, "Username and password are required.");

        if (await _users.UsernameExistsAsync(username))
            return (false, "Username already exists.");

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        await _users.AddAsync(user);
        return (true, "User registered successfully.");
    }
}