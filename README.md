# InventoryPro — Inventory Management System

A full-stack desktop application built with **C# / .NET 10 / WPF** as a final-year software engineering project.

## Tech Stack

| Layer      | Technology                              |
|------------|----------------------------------------|
| UI         | WPF (.NET 10), MVVM Pattern             |
| Database   | SQLite via Entity Framework Core 10     |
| Security   | BCrypt password hashing                 |
| Pattern    | Repository Pattern + Service Layer      |
| Language   | C# 13 with async/await throughout       |

## Features

- **Authentication** — Login with BCrypt-hashed passwords, role-based access (Admin/User)
- **Dashboard** — Live stats: total products, low stock alerts, categories, suppliers, recent transactions
- **Products** — Full CRUD with SKU, pricing, stock thresholds, category & supplier linking
- **Categories** — Manage product categories with duplicate name validation
- **Suppliers** — Manage supplier contacts and details
- **Stock Transactions** — Record Stock In / Stock Out / Adjustments with automatic quantity updates

## Getting Started

### Prerequisites
- Visual Studio 2026
- .NET 10 SDK

### Run
1. Clone the repo: `git clone <your-repo-url>`
2. Open `InventoryManagementSystem.sln` in Visual Studio
3. Press **F5** to build and run
4. Login with default credentials:
   - **Username:** `admin`
   - **Password:** `Admin@123`

> The SQLite database is auto-created at `%LocalAppData%\InventoryApp\inventory.db` on first run.

## Project Structure

InventoryApp/
├── Models/          # EF Core entity classes
├── Data/            # DbContext, generic repository, specific repositories
├── Services/        # AuthService (login/register), SessionManager
├── ViewModels/      # MVVM ViewModels — one per view
└── Views/           # WPF XAML views + code-behind

## Architecture

This project uses the **MVVM (Model-View-ViewModel)** pattern:
- **Models** define the database schema
- **Repositories** abstract all database operations (Repository Pattern)
- **Services** contain business logic (authentication, session)
- **ViewModels** expose data and commands to the UI
- **Views** are purely declarative XAML — no business logic in code-behind

## Default Credentials

| Username | Password  | Role  |
|----------|-----------|-------|
| admin    | Admin@123 | Admin |