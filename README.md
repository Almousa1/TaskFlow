# TaskFlow

A bilingual (English/Arabic) task management web application built with ASP.NET Core MVC. Organize projects, track tasks, and manage your workflow with a clean, modern interface.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_Core-MVC-512BD4?logo=dotnet)
![EF Core](https://img.shields.io/badge/EF_Core-8.0-512BD4?logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL_Server-2019-CC2927?logo=microsoft-sql-server)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?logo=bootstrap)

---

## Features

- **Dashboard** — Overview with stats cards (total, completed, pending, overdue tasks) and recent activity
- **Task Management** — Create, edit, delete, and toggle task completion; filter by status, project, or category; search by keyword; priority levels with color-coded tags
- **Project Management** — Organize tasks into color-coded projects
- **Category Management** — Classify tasks with color-coded categories
- **Admin Panel** — Manage user roles and task statuses (bilingual)
- **Bilingual Localization** — Full English/Arabic support with RTL layout and language switcher
- **Authentication & Authorization** — Cookie-based auth with role-based access (User / Admin)
- **Soft Delete** — Safe deletion with dependency cascading
- **Modern UI** — Custom design system with Swiss-modern aesthetics, animated counters, and hover-reveal actions

## Tech Stack

| Layer | Technology |
|---|---|
| **Framework** | .NET 8.0 (C# 12) |
| **Presentation** | ASP.NET Core MVC with Areas |
| **ORM** | Entity Framework Core 8.0.10 |
| **Database** | SQL Server (via EF Core SQL Server provider) |
| **Auth** | Cookie Authentication with role-based policies |
| **Frontend** | Razor Views, Bootstrap 5.3, Bootstrap Icons, jQuery Validation, SweetAlert2 |
| **Design** | Custom CSS with design tokens (Inter / Cairo fonts) |
| **Localization** | `.resx` resource files with `CookieRequestCultureProvider` |

## Architecture

The project follows a clean 4-layer architecture:

```
TaskFlow.sln
├── TaskFlow                # Presentation Layer (ASP.NET Core MVC)
│   ├── Areas/Auth          #   Login, SignUp
│   ├── Areas/Dashboard     #   CRUD controllers & views
│   ├── Controllers         #   Landing, Language, Home
│   ├── Resources           #   .resx localization files
│   └── wwwroot             #   Static assets (CSS, JS, lib)
├── TaskFlow.Business       # Business Logic Layer
│   ├── Domain              #   Domain services
│   └── ViewModels          #   ViewModel DTOs
├── TaskFlow.Data           # Data Access Layer
│   ├── Models              #   EF Core entity models
│   └── Repository          #   Generic repository + implementations
└── TaskFlow.Resources      # Shared Resources Layer
    └── Resources           #   Shared .resx files (ViewModels)
```

### Project dependencies

```
TaskFlow (Web) → TaskFlow.Business → TaskFlow.Data
                                  ↘ TaskFlow.Resources
```

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/) (LocalDB, Express, or full instance)
- Visual Studio 2022 (recommended) or any C# IDE

### Setup

1. Clone the repository:

   ```bash
   git clone https://github.com/your-org/TaskFlow.git
   cd TaskFlow
   ```

2. Update the connection string in `TaskFlow/appsettings.json`:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Data Source=localhost;Initial Catalog=TaskFlowDb;Integrated Security=True;Pooling=False;Encrypt=False;Trust Server Certificate=True"
   }
   ```

3. Restore and run:

   ```bash
   dotnet restore
   dotnet run --project TaskFlow
   ```

   The database is created and seeded automatically on first startup.

### Default Credentials

| Role | Email | Password |
|---|---|---|
| **Admin** | `admin@taskflow.local` | `Admin@123` |
| **User** | `user@taskflow.local` | `User@123` |

## Usage

### Public Area

- `GET /` — Landing page
- `GET /Auth/Login` — Login page
- `GET /Auth/SignUp` — User registration

### Dashboard (authenticated)

| Route | Description |
|---|---|
| `/Dashboard` | Overview with stats and recent tasks |
| `/Dashboard/TodoItem` | Manage tasks (CRUD, filter, search) |
| `/Dashboard/Project` | Manage projects |
| `/Dashboard/Category` | Manage categories |

### Admin-only

| Route | Description |
|---|---|
| `/Dashboard/Status` | Manage task statuses (bilingual) |
| `/Dashboard/UserRole` | Manage user roles |

### Language Switching

Add `?culture=ar` or `?culture=en` to any URL, or use the language switcher button in the interface. Preference is persisted via cookie.

## Project Structure

```
TaskFlow/
├── TaskFlow/                        # Web application
│   ├── Program.cs                   # Startup, DI, middleware, seeding
│   ├── appsettings.json             # Configuration
│   ├── Controllers/                 # Home, Landing, Language
│   ├── Areas/Auth/                  # Login, SignUp
│   │   ├── Controllers/
│   │   └── Views/
│   ├── Areas/Dashboard/             # Dashboard controllers & views
│   │   ├── Controllers/             # Dashboard, TodoItem, Project, Category, Status, UserRole
│   │   └── Views/
│   ├── Resources/                   # AuthResource, DashboardResource (.resx)
│   ├── wwwroot/
│   │   ├── css/taskflow.css         # Custom design system
│   │   ├── js/
│   │   └── lib/                     # Bootstrap, jQuery, validation
├── TaskFlow.Business/
│   ├── Domain/                      # Business logic services
│   └── ViewModels/                  # DTOs for views
├── TaskFlow.Data/
│   ├── Models/                      # EF Core entities
│   └── Repository/                  # Data access layer
├── TaskFlow.Resources/              # Shared resources
└── seed-data.sql                    # Sample data script
```

## Custom Design System

The UI uses a custom CSS design system defined in `taskflow.css` with:

- **Design tokens** via CSS custom properties (slate + emerald palette)
- **Inter** font for English, **Cairo** for Arabic
- **Swiss-modern** aesthetic: flat design, subtle borders, minimal shadows
- **Components**: Stats cards, data tables, tags, filter chips, action buttons, form controls, auth cards, empty states
- **RTL support** via `dir` attribute switching
- **Animations**: Fade-in page transitions, animated stat counters, hover-reveal row actions
