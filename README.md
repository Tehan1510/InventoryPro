# InventoryPro — Inventory Management System

A desktop inventory management application built with **C# / .NET 10 / WPF** using the MVVM pattern, Repository pattern, and SQLite via Entity Framework Core.

---

## Tech Stack

| Layer      | Technology                                |
|------------|------------------------------------------|
| UI         | WPF (.NET 10), MVVM Pattern              |
| Database   | SQLite via Entity Framework Core 10      |
| Security   | BCrypt password hashing (BCrypt.Net-Next) |
| Pattern    | Repository Pattern + Service Layer       |
| Language   | C# 13 with async/await                  |

---

## Features

- **Authentication** — BCrypt-hashed passwords, role-based access (Admin / User)
- **Dashboard** — Live stats: total products, low-stock alerts, categories, suppliers, recent transactions
- **Products** — Full CRUD with SKU, pricing, stock thresholds, category & supplier linking; search by name/SKU/description; color-coded low-stock badges; soft delete preserves transaction history
- **Categories** — CRUD with duplicate-name validation; blocks deletion if active products exist
- **Suppliers** — CRUD with contact details (name, email, phone, address)
- **Stock Transactions** — Record StockIn / StockOut / Adjustment; automatic quantity updates; StockOut validated against available stock

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2022 (v17.8+) or Visual Studio 2025 with the **.NET desktop development** workload

---

## Getting Started

```bash
# 1. Clone the repo
git clone <your-repo-url>
cd InventoryManagementSystem
```

**In Visual Studio:**
1. Open `InventoryManagementSystem.slnx`
2. Set `InventoryApp` as the startup project (right-click → Set as Startup Project)
3. Press **F5** to build and run

**From the command line:**
```bash
cd InventoryApp
dotnet run
```

On first launch the app auto-creates the SQLite database and seeds the admin account.

### Default Credentials

| Username | Password  | Role  |
|----------|-----------|-------|
| admin    | Admin@123 | Admin |

> Database location: `%LocalAppData%\InventoryApp\inventory.db`

---

## Project Structure

```
InventoryApp/
├── Models/                   # EF Core entity classes (Product, Category, Supplier, User, StockTransaction)
├── Data/
│   ├── AppDbContext.cs       # EF Core DbContext; configures SQLite path and seeds categories
│   ├── IRepository.cs        # Generic repository interface
│   ├── BaseRepository.cs     # Default CRUD implementation
│   └── Repositories/         # Entity-specific overrides (ProductRepository, etc.)
├── Services/
│   ├── AuthService.cs        # Login / register with BCrypt
│   └── SessionManager.cs     # Holds the current logged-in user (static)
├── ViewModels/
│   ├── BaseViewModel.cs      # INotifyPropertyChanged + SetProperty helper
│   ├── RelayCommand.cs       # ICommand implementation
│   ├── LoginViewModel.cs
│   ├── MainViewModel.cs      # Navigation + logout
│   ├── DashboardViewModel.cs
│   ├── ProductsViewModel.cs
│   ├── CategoriesViewModel.cs
│   ├── SuppliersViewModel.cs
│   └── StockViewModel.cs
└── Views/                    # WPF XAML views + minimal code-behind
    ├── App.xaml / App.xaml.cs
    ├── LoginWindow.xaml
    ├── MainWindow.xaml
    ├── DashboardView.xaml
    ├── ProductsView.xaml
    ├── CategoriesView.xaml
    ├── SuppliersView.xaml
    └── StockView.xaml
```

---

## Architecture

**MVVM** — Views contain zero business logic. All state and commands live in ViewModels.

**Repository Pattern** — `BaseRepository<T>` provides default CRUD. Each entity repository overrides only what it needs (e.g., `ProductRepository` adds search, soft-delete, and eager-loading of navigation properties).

**Navigation** — `MainViewModel.CurrentViewModel` holds the active page ViewModel. `MainWindow.xaml` maps each ViewModel type to its View via `DataTemplate`, so `ContentControl` renders the correct view automatically.

**Startup flow:**
```
App.OnStartup
  → EnsureCreatedAsync()   (creates DB + schema on first run)
  → seed admin user        (if Users table is empty)
  → show LoginWindow
      → on success → show MainWindow → navigate to Dashboard
      → on logout → show LoginWindow again
```

---

## What to Test

### Authentication
- Log in with `admin / Admin@123`
- Try wrong password → error message shown, login blocked
- Log out → returns to login screen

### Products
- Add a product (name and category are required)
- Search by name, SKU, or description → list filters live
- Edit a product and save
- Delete a product → it disappears from the list but its stock transactions remain

### Categories
- Add a category with a duplicate name → blocked with error
- Try deleting a category that has active products → blocked with error
- Delete a category with no products → succeeds

### Suppliers
- Add, edit, and delete suppliers

### Stock Transactions
- Record a **StockIn** → product quantity increases
- Record a **StockOut** with quantity > available stock → blocked with error
- Record a **StockOut** within available stock → quantity decreases
- Record an **Adjustment** → sets quantity to the entered value (absolute, not a delta)
- Check the Dashboard — recent transactions and counters should update (press Refresh)

### Dashboard
- After adding products, Total Products counter increases
- After stock drops below threshold, Low Stock Items counter increases
- Press **Refresh** to reload all counters

---

## Notes

- The database is **not** migrated automatically if you change the schema after first run. Delete `%LocalAppData%\InventoryApp\inventory.db` to reset.
- `PasswordBox` is bound via code-behind (`LoginWindow.xaml.cs`) — WPF intentionally blocks direct XAML binding on `PasswordBox` for security reasons.
- Global styles (buttons, inputs, cards) are defined in `App.xaml` and referenced by key in all views.
